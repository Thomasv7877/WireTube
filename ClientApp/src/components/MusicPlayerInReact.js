import React, { useState, useEffect, useRef } from 'react';
import '../custom.css'

const MusicPlayerInReact = () => {
  const [musicFiles, setMusicFiles] = useState([]);
  const [currentSongIndex, setCurrentSongIndex] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const audioRef = useRef(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredSongs, setFilteredSongs] = useState([]);

  const fetchMusicList = async () => {
    try {
      const response = await fetch('/ytApi');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      setMusicFiles(data);
      setFilteredSongs(data)
      //handleSearch(data)
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
    setCurrentSongIndex(prevIndex => (prevIndex > 0 ? prevIndex - 1 : filteredSongs.length - 1));
    setIsPlaying(true);
  };

  const handleNext = () => {
    setCurrentSongIndex(prevIndex => (prevIndex < filteredSongs.length - 1 ? prevIndex + 1 : 0));
    setIsPlaying(true);
  };
  const handleDoubleClick = (index) => {
    setCurrentSongIndex(index);
    setIsPlaying(true);
  }
  const handleSearch = (event) => {
    const query = event.target.value;
    setSearchQuery(query);
    setIsPlaying(false);
  
    const filtered = musicFiles.filter((song) =>
      song.toLowerCase().includes(query.toLowerCase())
    );
    setFilteredSongs(filtered);
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
  }, [currentSongIndex, isPlaying, filteredSongs]);

  return (
    <div className='al-container'>
      {musicFiles.length > 0 ? (
        <>
          <input id='search-lib' type="text" value={searchQuery} onChange={handleSearch} placeholder="Search songs..."/>
          <div className='scroll-pane'>
            <div className='content-wrapper'>
          <ol id='audio-library'>
            {filteredSongs.map((file, index) => (
              <li key={index} onDoubleClick={() => handleDoubleClick(index)} className={currentSongIndex === index && isPlaying ? 'active' : ''}>{file}</li>
            ))}
          </ol>
          </div>
          </div>
          
          <div id='media-control'>
          <audio id="audio-player" ref={audioRef} src={isPlaying? `ytApi/audio?fileName=${filteredSongs[currentSongIndex]}` : null} controls />
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
