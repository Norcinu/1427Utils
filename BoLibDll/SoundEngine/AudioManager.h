#ifndef AUDIOMANAGER_H
#define AUDIOMANAGER_H

//------AudioManager------//
/*AudioManager keeps track of all the
audio samples loaded into the game, from
here they can be easily accessed by name.*/
//------------------------//

#define DIRECTSOUND_VERSION 0x0900

#include <map>
#include <string>
#include <Windows.h>
#include "AudioSample.h"
#include "NonCopyable.h"
#include "Singleton.h"
#include "../General.h"


#define DLLEXPORT extern "C" __declspec(dllexport)

DLLEXPORT void loadAndPlayFile(const char* filename);
DLLEXPORT void clearSoundResources();

typedef std::map<std::string, AudioSample*> AudioSamples;

class AudioManager : public NonCopyable
{
private:
	AudioManager();
	friend class Singleton<AudioManager>;
	

public:
	~AudioManager();

	void SetVolume(unsigned int volume);
	void StopAllSounds();
	AudioSample* GetAudioSample(const std::string& name);
	bool Initialize(HWND hwnd);
	bool LoadAudio();
	bool LoadAudio(const char* fname);
	IDirectSound8* GetDirectSound() const;
	unsigned int GetVolume() const;

	void CheckPerformanceVolumeChanged(void);
	void SetServerBasedGame(unsigned char type);

	void CleanUp();
	AudioSamples GetAudioSamples() const;

private:
	AudioSamples m_audioSamples;
	IDirectSound8* m_directSound;
	IDirectSoundBuffer* m_primaryBuffer;
	unsigned int m_volume;
	unsigned int SavedMasterVolume;
	int	ServerBasedGame;
};

typedef Singleton<AudioManager> TheAudioManager;

#endif AUDIOMANAGER_H 