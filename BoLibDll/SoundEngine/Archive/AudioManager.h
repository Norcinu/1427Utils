#ifndef	AUDIOMANAGER_H
#define	AUDIOMANAGER_H

#include <map>
#include <vector>
#include "ErrorLog.h"
#include "Factory.h"
#include "PlatformSpecifics.h"

class AudioSample;
class DisplaySystem;
class ProcessManager;
class XMLElement;

typedef enum
{
	AUDIOTYPE_DSOUND = 0
}AUDIOTYPE;

class AudioManager
{
protected:
	typedef PFactory<unsigned int, AudioSample, AudioManager> AudioSampleFactory;
public:
	AudioManager();
	virtual ~AudioManager();
	void CleanUp();
	void Pause(const std::string& audioName);
	void PauseAll();
	void Play(const std::string& audioName, bool loop = false, unsigned int volumeModifier = 100);
	virtual void Register(AudioSampleFactory* audioSampleFactory) = 0;
	void Resume(const std::string& audioName);
	void ResumeAll();
	void SetMasterVolume(unsigned int masterVolume);
	void Stop(const std::string& audioName);
	void StopAll();
	AudioSample* GetAudioSample(const std::string& audioName);
	AudioSampleFactory& GetFactory(){return m_audioSampleFactory;}
	virtual AUDIOTYPE GetAudioManagerType() const = 0;
	bool HasAudioSample(const std::string& audioName);
	bool IsPlaying(const std::string& audioName);
	DLL_LINK GetDLLLink() const;
	ERR_TYPE AddAudioSample(AudioSample* audioSample);
	virtual ERR_TYPE Initialize(XMLElement* audioSettings, DisplaySystem* displaySystem) = 0;
	ERR_TYPE LoadAudio(XMLElement* audioData);
	unsigned int GetMasterVolume() const;
protected:
	typedef std::map<std::string, AudioSample*> AudioSamples;
	typedef std::vector<AudioSample*> PausedSamples;
	AudioSamples m_audioSamples;
	AudioSampleFactory m_audioSampleFactory;
	DLL_LINK m_dllLink;
	PausedSamples m_pausedSamples;
	ProcessManager* m_processManager;
	unsigned int m_masterVolume;
};

#endif AUDIOMANAGER_H