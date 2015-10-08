#include "AudioManager.h"
#include "AudioSample.h"
#include "ProcessManager.h"
#include "XMLRead.h"

/**
* Default Constructor
*/
AudioManager::AudioManager()
{
	m_dllLink = NULL;
	m_processManager = nullptr;
	m_masterVolume = 100;
}

/**
* Default Destructor
*/
AudioManager::~AudioManager()
{
	CleanUp();
}

/**
* This function will clean up the AudioManager.
*/
void AudioManager::CleanUp()
{
	if(m_processManager)
	{
		delete m_processManager;
		m_processManager = nullptr;
	}

	for(AudioSamples::iterator it = m_audioSamples.begin(); it != m_audioSamples.end(); ++it)
	{
		delete it->second;
		it->second = nullptr;
	}
	m_audioSamples.clear();
	m_pausedSamples.clear();
}

/**
* Pauses the sound clip if it is currently playing.
* @param audioName The name of the AudioSample you wish to pause.
*/
void AudioManager::Pause(const std::string& audioName)
{
	AudioSample* audioSample = GetAudioSample(audioName);

	if(audioSample->IsPlaying())
	{
		audioSample->Pause();
	}
}

/**
* Pauses any currently playing sound, this can then be resumed by calling ResumeSounds().
*/
void AudioManager::PauseAll()
{
	for(AudioSamples::iterator it = m_audioSamples.begin(); it != m_audioSamples.end(); ++it)
	{
		if(it->second->IsPlaying())
		{
			it->second->Pause();
			m_pausedSamples.push_back(it->second);
		}
	}
}

/**
* Plays the sound clip.
* @param audioName The name of the AudioSample you wish to play.
* @param loop If this value is true, then the sound clip will loop infinitely until it is stopped.
* @param volumeModifier This is a value in the range 0-100 that will modify the AudioManagers master volume sound level.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
void AudioManager::Play(const std::string& audioName, bool loop, unsigned int volumeModifier)
{
	GetAudioSample(audioName)->Play(loop, volumeModifier);
}

/**
* Resumes the sound clip if it is has been paused.
* @param audioName The name of the AudioSample you wish to resume.
*/
void AudioManager::Resume(const std::string& audioName)
{
	GetAudioSample(audioName)->Resume();
}

/**
* Resumes any currently paused sound.
*/
void AudioManager::ResumeAll()
{
	for(unsigned int i = 0; i < m_pausedSamples.size(); i++)
	{
		m_pausedSamples[i]->Resume();
	}
	m_pausedSamples.clear();
}

/**
* Sets the new master volume sound level, this calls ChildSetMasterVolume so the DLL can carry out this change.
* @param masterVolume The new volume level on a percentage scale from 0-100.
*/
void AudioManager::SetMasterVolume(unsigned int masterVolume)
{
	m_masterVolume = masterVolume;
}

/**
* Stops the sound clip if it is currently playing.
* @param audioName The name of the AudioSample you wish to stop.
*/
void AudioManager::Stop(const std::string& audioName)
{
	AudioSample* audioSample = GetAudioSample(audioName);

	if(audioSample->IsPlaying())
	{
		audioSample->Stop();
	}
}

/**
* Stops any currently playing sounds.
*/
void AudioManager::StopAll()
{
	for(AudioSamples::iterator it = m_audioSamples.begin(); it != m_audioSamples.end(); ++it)
	{
		if(it->second->IsPlaying())
		{
			it->second->Stop();
		}
	}
}

/**
* @param audioName The name of the AudioSample you wish to retrive.
* @return The AudioSample you have specified or nullptr if the AudioSample does not exist.
*/
AudioSample* AudioManager::GetAudioSample(const std::string& audioName)
{
	AudioSamples::iterator it = m_audioSamples.find(audioName);
	if(it != m_audioSamples.end())
	{
		return it->second;
	}
	return nullptr;
}

/**
* @param audioName The name of the AudioSample to be checked.
* @return Returns true if the AudioSample has been loaded into the AudioManager.
*/
bool AudioManager::HasAudioSample(const std::string& audioName)
{
	AudioSamples::iterator it = m_audioSamples.find(audioName);
	if(it != m_audioSamples.end())
	{
		return true;
	}
	return false;
}

/**
* @param audioName The name of the AudioSample you wish to check.
* @return Returns true if the sample is playing.
*/
bool AudioManager::IsPlaying(const std::string& audioName)
{
	return GetAudioSample(audioName)->IsPlaying();
}

/**
* @return Retrieves the DLL Handle used to Link VizTech to a derived AudioManager.
*/
DLL_LINK AudioManager::GetDLLLink() const
{
	return m_dllLink;
}

/**
* Adds an AudioSample to the AudioManager, also checks to see if an AudioSample of this name already exists.
* @param audioSample The AudioSample to be added to the AudioManager.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
ERR_TYPE AudioManager::AddAudioSample(AudioSample* audioSample)
{
	if(HasAudioSample(audioSample->GetName()))
	{
		return ReportError(ERR_WARNING, "File: %s|Line: %i|The AudioSample \"%s\"|Cannot Be Added To The AudioManager|As An AudioSample Already Exists With This Name Already", __FILE__, __LINE__, audioSample->GetName().c_str());
	}

	m_audioSamples[audioSample->GetName()] = audioSample;
	return ERR_NONE;
}

/**
* Function called by VizTechLib once the AudioManager has been Initialized.
* @param audioData The XML Node containing information regarding the individual audio clips to be created.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
ERR_TYPE AudioManager::LoadAudio(XMLElement* audioData)
{
	if(audioData->IsNode("Samples"))
	{
		audioData = audioData->GetFirstXMLChild("Sample");
	}

	if(!audioData->IsNode("Sample"))
	{
		return ReportError(ERR_WARNING, "File: %s|Line: %i|Unable To Load Audio As A <Sample> Node Does Not Exist", __FILE__, __LINE__);
	}

	while(audioData)
	{
		if(audioData->IsNode("Sample"))
		{
			AudioSample* audioSample = GetFactory().Create(GetAudioManagerType(), this);
			if(!audioSample)
			{
				return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Creating AudioSample", __FILE__, __LINE__);
			}

			ERR_TYPE errorType = audioSample->Load(audioData);
			if(ERRCHK(errorType))
			{
				delete audioSample;
				audioSample = nullptr;
				return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed Loading AudioSample", __FILE__, __LINE__);
			}

			errorType = AddAudioSample(audioSample);
			if(ERRCHK(errorType))
			{
				delete audioSample;
				audioSample = nullptr;
				return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed To Add AudioSample %s", __FILE__, __LINE__, audioSample->GetName().c_str());
			}
		}
		audioData = audioData->GetNextXMLSibling();
	}
	return ERR_NONE;
}

/**
* @return Retrieves the master volume level on a percentage scale from 0-100.
*/
unsigned int AudioManager::GetMasterVolume() const
{
	return m_masterVolume;
}