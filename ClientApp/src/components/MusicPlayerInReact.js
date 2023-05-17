import React, { useState, useEffect, useRef } from 'react';

const MusicPlayerInReact = () => {
  const [musicFiles, setMusicFiles] = useState([]);
  const [currentSongIndex, setCurrentSongIndex] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const audioRef = useRef(null);

  const fetchMusicList = async () => {
    try {
      const response = await fetch('/ytApi');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      setMusicFiles(data);
      setCurrentSongIndex(0);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  const handlePlay = () => {
    setIsPlaying(true);
  };

  const handlePause = () => {
    setIsPlaying(false);
  };

  const handlePrevious = () => {
    setCurrentSongIndex(prevIndex => (prevIndex > 0 ? prevIndex - 1 : musicFiles.length - 1));
    setIsPlaying(true);
  };

  const handleNext = () => {
    setCurrentSongIndex(prevIndex => (prevIndex < musicFiles.length - 1 ? prevIndex + 1 : 0));
    setIsPlaying(true);
  };

  // Fetch music files when the component mounts
  useEffect(() => {
    fetchMusicList();
  }, []);

  // Handle playback when the current song index changes
  useEffect(() => {
    if (currentSongIndex !== null) {
      if (isPlaying) {
        // Play the audio
        // Assumes musicFiles contain the full URLs to the audio files
        //const audio = new Audio(`ytApi/audio?fileName=${musicFiles[currentSongIndex]}`);
        //const audio = document.getElementById('audio-player');
        //audio.play();
        audioRef.current.play();
      } else {
        // Pause the audio
        //const audio = document.getElementById('audio-player');
        //audio.pause();
        audioRef.current.pause();
      }
    }
  }, [currentSongIndex, isPlaying, musicFiles]);

  return (
    <div>
      {musicFiles.length > 0 ? (
        <>
          <audio id="audio-player" ref={audioRef} src={`ytApi/audio?fileName=${musicFiles[currentSongIndex]}`} />
          <ul>
            {musicFiles.map((file, index) => (
              <li key={index}>{file}</li>
            ))}
          </ul>

          <div>
            <button onClick={handlePrevious}>Previous</button>

            {isPlaying ? (
              <button onClick={handlePause}>Pause</button>
            ) : (
              <button onClick={handlePlay}>Play</button>
            )}

            <button onClick={handleNext}>Next</button>
          </div>
        </>
      ) : (
        <p>Loading music files...</p>
      )}
    </div>
  );
};

export default MusicPlayerInReact;
