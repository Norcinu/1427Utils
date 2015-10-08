#include "AudioSample.h"
#include "XMLRead.h"

/**
* Default Constructor
*/
AudioSample::AudioSample(AudioManager* audioManager)
{
	m_audioManager = audioManager;
	m_looping = false;
	m_name = "";
	m_volumeModifier = 100;
}

/**
* Default Destuctor
*/
AudioSample::~AudioSample()
{

}

/**
* Loads in attributes associated to the AudioSample.
* @param sampleSettings This is an XMLElement describing specific settings for the AudioSample.
* @return Returns ERR_TYPE to handle any errors that could occur.
*/
ERR_TYPE AudioSample::Load(XMLElement* sampleSettings)
{
	sampleSettings->GetString("Name", m_name);

	std::string filename = "";
	sampleSettings->GetString("Filename", filename);
	if(!filename.size())
	{
		return ReportError(ERR_FATAL, "File: %s|Line: %i|Failed To Load AudioSample As No Filename Has Been Provided", __FILE__, __LINE__);
	}

	if(!m_name.size())
	{
		m_name = filename;
	}

	filename = XMLRead::GetFilename(FILEPREPEND_WAV, filename);

	return LoadAudio(filename, sampleSettings);
}

/**
* @return Returns the name of the AudioSample.
*/
const std::string& AudioSample::GetName() const
{
	return m_name;
}