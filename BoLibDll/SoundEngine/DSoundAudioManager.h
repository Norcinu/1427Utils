#ifndef	DSOUNDAUDIOMANAGER_H
#define	DSOUNDAUDIOMANAGER_H

#define DIRECTSOUND_VERSION 0x0900

#include "..\AudioManager.h"

struct IDirectSound8;
struct IDirectSoundBuffer;

class DSoundAudioManager : public AudioManager
{
public:
	DSoundAudioManager(DLL_LINK dllLink);
	~DSoundAudioManager();
	void Register(AudioSampleFactory* audioSampleFactory) override;
	AUDIOTYPE GetAudioManagerType() const override;
	ERR_TYPE Initialize(XMLElement* audioSettings, DisplaySystem* displaySystem) override;
	IDirectSound8* GetDirectSound() const;
	unsigned int GetRangeMultiplier() const;
private:
	IDirectSound8* m_directSound;
	IDirectSoundBuffer* m_primaryBuffer;
	unsigned int m_rangeMultiplier;
};

#endif DSOUNDAUDIOMANAGER_H