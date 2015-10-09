#include <dsound.h>
#include "..\DisplaySystem.h"
#include "DSoundAudioManager.h"
#include "DSoundAudioSample.h"
#include "..\XMLRead.h"

/**
* DLL Create & Release Functions
*/
ERR_TYPE CreateAudioManager(HINSTANCE dll, AudioManager** audioManager, ErrorHandler** errorHandler)
{
	if(!*audioManager)
	{
		*audioManager = new DSoundAudioManager(dll);
		if(*errorHandler)
		{
			SetErrorHandler(*errorHandler);
		}

		return ERR_NONE;
	}
	return ERR_FATAL;
}

ERR_TYPE ReleaseAudioManager(AudioManager** audioManager)
{
	if(!*audioManager)
	{
		return ERR_FATAL;
	}
	delete *audioManager;
	*audioManager = nullptr;
	return ERR_NONE;
}

/**
* Default Constructor.
*/
DSoundAudioManager::DSoundAudioManager(DLL_LINK dllLink)
{
	m_dllLink = dllLink;
	m_directSound = nullptr;
	m_primaryBuffer = nullptr;
	m_rangeMultiplier = 35;
}

/**
* Default Deconstructor
*/
DSoundAudioManager::~DSoundAudioManager()
{
	CleanUp();

	if(m_primaryBuffer)
	{
		m_primaryBuffer->Release();
		m_primaryBuffer = nullptr;
	}

	if(m_directSound)
	{
		m_directSound->Release();
		m_directSound = nullptr;
	}
}

/**
* Registers Audio Specific information, to allow correct creation at runtime.
* @param audioSampleFactory This allows default DSound types to be registered to a Factory.
*/
void DSoundAudioManager::Register(AudioSampleFactory* audioSampleFactory)
{
	audioSampleFactory->Register(AUDIOTYPE_DSOUND, new DerivedPCreator<DSoundAudioSample, AudioSample, AudioManager>);
}

/**
* @return Returns the audio type of AUDIOTYPE_DSOUND.
*/
AUDIOTYPE DSoundAudioManager::GetAudioManagerType() const
{
	return AUDIOTYPE_DSOUND;
}

/**
* Function called by VizTechLib once the DisplaySystem has been set-up.
* @param audioSettings This is an XMLElement describing specific settings for the AudioManager.
* @param displaySystem The DisplaySystem used by the game.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
ERR_TYPE DSoundAudioManager::Initialize(XMLElement* audioSettings, DisplaySystem* displaySystem)
{
	if(audioSettings)
	{
		audioSettings->GetUnsignedInt("RangeMultiplier", &m_rangeMultiplier);
	}

	if(FAILED(DirectSoundCreate8(NULL, &m_directSound, NULL)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Creating DirectSound", __FILE__, __LINE__);
	}

	if(FAILED(m_directSound->SetCooperativeLevel(displaySystem->GetScreen(0)->GetScreenHandle(), DSSCL_PRIORITY)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Setting Cooperative Level DirectSound", __FILE__, __LINE__);
	}

	DSBUFFERDESC buffer;
	buffer.dwBufferBytes = 0;
	buffer.dwFlags = DSBCAPS_STICKYFOCUS | DSBCAPS_PRIMARYBUFFER | DSBCAPS_CTRLVOLUME;
	buffer.dwReserved = 0;
	buffer.dwSize = sizeof(DSBUFFERDESC);
	buffer.guid3DAlgorithm = GUID_NULL;
	buffer.lpwfxFormat = NULL;

	if(FAILED(m_directSound->CreateSoundBuffer(&buffer, &m_primaryBuffer, NULL)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Creating Primary Sound Buffer", __FILE__, __LINE__);
	}

	WAVEFORMATEX waveFormat;
	waveFormat.cbSize = 0;
	waveFormat.nChannels = 2;
	waveFormat.nSamplesPerSec = 44100;
	waveFormat.wBitsPerSample = 16;
	waveFormat.wFormatTag = WAVE_FORMAT_PCM;
	waveFormat.nBlockAlign = (waveFormat.wBitsPerSample / 8) * waveFormat.nChannels;
	waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

	if(FAILED(m_primaryBuffer->SetFormat(&waveFormat)))
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Setting Wave Format", __FILE__, __LINE__);
	}
	return ERR_NONE;
}

/**
* @return Retrieves the DirectSound interface.
*/
IDirectSound8* DSoundAudioManager::GetDirectSound() const
{
	return m_directSound;
}

/**
* @return Retrieves the range multiplier required by DirectSound.
*/
unsigned int DSoundAudioManager::GetRangeMultiplier() const
{
	return m_rangeMultiplier;
}