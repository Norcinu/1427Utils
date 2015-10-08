#ifndef	DSOUNDAUDIOSAMPLE_H
#define	DSOUNDAUDIOSAMPLE_H

#include "..\AudioSample.h"

struct IDirectSoundBuffer8;

struct WaveHeader
{
	char m_chunkId[4];
	unsigned long m_chunkSize;
	char m_format[4];
	char m_subChunkId[4];
	unsigned long m_subChunkSize;
	unsigned short m_audioFormat;
	unsigned short m_numChannels;
	unsigned long m_sampleRate;
	unsigned long m_bytesPerSecond;
	unsigned short m_blockAlign;
	unsigned short m_bitsPerSample;
	char m_dataChunkId[4];
	unsigned long m_dataSize;
};

class DSoundAudioSample : public AudioSample
{
public:
	DSoundAudioSample(AudioManager* audioManager);
	~DSoundAudioSample();
	void Pause() override;
	void Play(bool loop = false, unsigned int volumeModifier = 100) override;
	void Resume() override;
	void Stop() override;
	bool IsPlaying() const override;
private:
	ERR_TYPE LoadAudio(const std::string& filename, XMLElement* sampleSettings) override;
private:
	DWORD m_pausePosition;
	IDirectSoundBuffer8* m_secondaryBuffer;
};

#endif DSOUNDAUDIOSAMPLE_H