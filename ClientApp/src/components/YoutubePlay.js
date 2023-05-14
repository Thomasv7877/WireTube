import React, { useState, useEffect} from "react";
import MusicPlayer from "./MusicPlayer";

// psotman test: [GET]https://localhost:7066/YtApi

export function YoutubePlay(){
    //var songs = [];
    const [songs, setSongs] = useState([]);

    
    useEffect(() => {
      const fetchSongs = async () => {
        const response = await fetch('/ytApi');
        const data = await response.json();
        setSongs(data);
    };
    fetchSongs();
    }, [])
    
      
    return(
        <><p>Playlist</p>
        {/*<ol>
          {songs.map((index, key) =>
            <li key={key}>{index}</li>
          )}
        </ol>*/}
        <MusicPlayer/>
      </>
    );
}