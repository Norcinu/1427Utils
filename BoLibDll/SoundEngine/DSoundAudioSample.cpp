#include <dsound.h>
#include "DSoundAudioManager.h"
#include "DSoundAudioSample.h"
#include "..\XMLRead.h"

/**
* Default Constructor.
*/
DSoundAudioSample::DSoundAudioSample(AudioManager* audioManager) : AudioSample(audioManager)
{
	m_secondaryBuffer = nullptr;
	m_pausePosition = 0;
}

/**
* Default Destructor.
*/
DSoundAudioSample::~DSoundAudioSample()
{
	if(m_secondaryBuffer)
	{
		m_secondaryBuffer->Release();
		m_secondaryBuffer = nullptr;
	}
}

/**
* Pauses the current sound clip.
*/
void DSoundAudioSample::Pause()
{
	m_pausePosition = 0;
	m_secondaryBuffer->GetCurrentPosition(&m_pausePosition, NULL);
	m_secondaryBuffer->Stop();
}

/**
* Plays the sound clip.
* @param loop If this value is true, then the sound clip will loop infinitely until it is stopped.
* @param volumeModifier This is a value in the range 0-100 that will modify the AudioManagers master volume sound level.
*/
void DSoundAudioSample::Play(bool loop, unsigned int volumeModifier)
{
	if(m_audioManager->GetMasterVolume())
	{
		DSoundAudioManager* dSoundAudio = static_cast<DSoundAudioManager*>(m_audioManager);

		m_secondaryBuffer->SetCurrentPosition(0);
		float volume = (float)m_audioManager->GetMasterVolume() * ((float)volumeModifier / 100.0f);

		if(FAILED(m_secondaryBuffer->SetVolume(unsigned int((volume - 100.0f) * dSoundAudio->GetRangeMultiplier()))))
		{
			ReportError(ERR_WARNING, "File: %s|Line: %i|Could Not Set Volume For AudioSample '%s'", __FILE__, __LINE__, m_name.c_str());
		}

		m_looping = loop;
		DWORD flags = 0;
		if(m_looping)
		{
			flags = DSBPLAY_LOOPING;
		}

		if(FAILED(m_secondaryBuffer->Play(0, 0, flags)))
		{
			ReportError(ERR_WARNING, "File: %s|Line: %i|Could Not Play AudioSample '%s'", __FILE__, __LINE__, m_name.c_str());
		}
	}
}

/**
* Pauses the current sound clip after a pause state.
*/
void DSoundAudioSample::Resume()
{
	m_secondaryBuffer->SetCurrentPosition(m_pausePosition);
	DWORD flags = 0;
	if(m_looping)
	{
		flags = DSBPLAY_LOOPING;
	}

	if(FAILED(m_secondaryBuffer->Play(0, 0, flags)))
	{
		ReportError(ERR_WARNING, "File: %s|Line: %i|Could Not Play AudioSample '%s'", __FILE__, __LINE__, m_name.c_str());
	}
}

/**
* Stops the current sound clip.
*/
void DSoundAudioSample::Stop()
{
	if(FAILED(m_secondaryBuffer->Stop()))
	{
		ReportError(ERR_WARNING, "File: %s|Line: %i|Could Not Stop AudioSample '%s'", __FILE__, __LINE__, m_name.c_str());
	}
}

/**
* @return Returns true if the current sample is playing.
*/
bool DSoundAudioSample::IsPlaying() const
{
	DWORD status;
	m_secondaryBuffer->GetStatus(&status);

	if(status &= DSBSTATUS_PLAYING)
	{
		return true;
	}
	return false;
}

/**
* Loads in attributes associated to the DSoundAudioSample.
* @param filename The filename and path needed to load the AudioSample.
* @param sampleSettings This is an XMLElement describing specific settings for the DSoundAudioSample.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
ERR_TYPE DSoundAudioSample::LoadAudio(const std::string& filename, XMLElement* sampleSettings)
{
	DSoundAudioManager* dSoundAudio = static_cast<DSoundAudioManager*>(m_audioManager);

	FILE* filePtr = 0;
	int error = fopen_s(&filePtr, filename.c_str(), "rb");
	if(error)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed To Open AudioSample '%s'", __FILE__, __LINE__, filename.c_str());
	}

	WaveHeader waveHeader;
	unsigned int count = fread(&waveHeader, sizeof(WaveHeader), 1, filePtr);
	if(count != 1)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed To Read WaveHeader For AudioSample '%s'", __FILE__, __LINE__, filename.c_str());
	}

	if((waveHeader.m_chunkId[0] != 'R') || (waveHeader.m_chunkId[1] != 'I') || (waveHeader.m_chunkId[2] != 'F') || (waveHeader.m_chunkId[3] != 'F'))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|The Chunk ID Of AudioSample '%s'|Is Not In The RIFF Format", __FILE__, __LINE__, filename.c_str());
	}

	if((waveHeader.m_format[0] != 'W') || (waveHeader.m_format[1] != 'A') || (waveHeader.m_format[2] != 'V') || (waveHeader.m_format[3] != 'E'))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s' Is Not Of WAVE Format", __FILE__, __LINE__, filename.c_str());
	}

	if((waveHeader.m_subChunkId[0] != 'f') || (waveHeader.m_subChunkId[1] != 'm') || (waveHeader.m_subChunkId[2] != 't') || (waveHeader.m_subChunkId[3] != ' '))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|The SubChunk ID Of AudioSample '%s'|Is Not 'fmt'", __FILE__, __LINE__, filename.c_str());
	}

	if(waveHeader.m_audioFormat != WAVE_FORMAT_PCM)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s'|Is Not Of WAVE_FORMAT_PCM Format", __FILE__, __LINE__, filename.c_str());
	}

	if(waveHeader.m_numChannels != 2 && waveHeader.m_numChannels != 1)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s'|Was Not Recorded In Mono Or Stereo Format", __FILE__, __LINE__, filename.c_str());
	}

	if(waveHeader.m_sampleRate != 44100)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s'|Was Not Recorded At A Sample Rate Of 44.1KHz", __FILE__, __LINE__, filename.c_str());
	}

	if(waveHeader.m_bitsPerSample != 16)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s'|Was Not Recorded In 16-Bit Format", __FILE__, __LINE__, filename.c_str());
	}

	if((waveHeader.m_dataChunkId[0] != 'd') || (waveHeader.m_dataChunkId[1] != 'a') || (waveHeader.m_dataChunkId[2] != 't') || (waveHeader.m_dataChunkId[3] != 'a'))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|The DataChunk Of AudioSample '%s'|Is Not 'data'", __FILE__, __LINE__, filename.c_str());
	}

	WAVEFORMATEX waveFormat;
	waveFormat.cbSize = 0;
	waveFormat.nChannels = waveHeader.m_numChannels;
	waveFormat.nSamplesPerSec = 44100;
	waveFormat.wBitsPerSample = 16;
	waveFormat.wFormatTag = WAVE_FORMAT_PCM;
	waveFormat.nBlockAlign = (waveFormat.wBitsPerSample / 8) * waveFormat.nChannels;
	waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

	DSBUFFERDESC buffer;
	buffer.dwBufferBytes = waveHeader.m_dataSize;
	buffer.dwFlags = DSBCAPS_GLOBALFOCUS | DSBCAPS_CTRLVOLUME | DSBCAPS_LOCDEFER;
	buffer.dwReserved = 0;
	buffer.dwSize = sizeof(DSBUFFERDESC);
	buffer.guid3DAlgorithm = GUID_NULL;
	buffer.lpwfxFormat = &waveFormat;

	IDirectSoundBuffer* tempBuffer;

	if(FAILED(dSoundAudio->GetDirectSound()->CreateSoundBuffer(&buffer, &tempBuffer, NULL)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Creating Temporary Buffer For AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}

	if(FAILED(tempBuffer->QueryInterface(IID_IDirectSoundBuffer8, (void**)&m_secondaryBuffer)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Creating Secondary Buffer For AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}
	tempBuffer->Release();
	tempBuffer = 0;

	fseek(filePtr, sizeof(WaveHeader), SEEK_SET);
	unsigned char* waveData = new unsigned char[waveHeader.m_dataSize];

	if(!waveData)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|AudioSample '%s' Has No Wave Data", __FILE__, __LINE__, filename.c_str());
	}

	count = fread(waveData, 1, waveHeader.m_dataSize, filePtr);

	if(count != waveHeader.m_dataSize)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Reading Wave Data For AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}

	error = fclose(filePtr);
	if(error)
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Closing AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}

	unsigned char* bufferPtr;
	unsigned long bufferSize;

	if(FAILED(m_secondaryBuffer->Lock(0, waveHeader.m_dataSize, (void**)&bufferPtr, (DWORD*)&bufferSize, NULL, 0, 0)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Locking Secondary Buffer Of AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}

	memcpy(bufferPtr, waveData, waveHeader.m_dataSize);

	if(FAILED(m_secondaryBuffer->Unlock((void*)bufferPtr, bufferSize, NULL, 0)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Unlocking Secondary Buffer Of AudioSample '%s' Failed", __FILE__, __LINE__, filename.c_str());
	}

	delete [] waveData;
	waveData = 0;

	return ERR_NONE;
}