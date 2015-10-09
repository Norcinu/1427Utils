#ifndef	AUDIOSAMPLE_H
#define	AUDIOSAMPLE_H

#include <string>
#include "ErrorLog.h"

class AudioManager;
class XMLElement;

class AudioSample
{
public:
	AudioSample(AudioManager* audioManager);
	virtual ~AudioSample();
	virtual void Pause() = 0;
	virtual void Play(bool loop = false, unsigned int volumeModifier = 100) = 0;
	virtual void Resume() = 0;
	virtual void Stop() = 0;
	virtual bool IsPlaying() const = 0;
	ERR_TYPE Load(XMLElement* sampleSettings);
	const std::string& GetName() const;
protected:
	virtual ERR_TYPE LoadAudio(const std::string& filename, XMLElement* sampleSettings) = 0;
protected:
	AudioManager* m_audioManager;
	bool m_looping;
	std::string m_name;
	unsigned int m_volumeModifier;
};

#endif AUDIOSAMPLE_H