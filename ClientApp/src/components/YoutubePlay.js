import React, { useState, useEffect} from "react";
//import MusicPlayer from "./MusicPlayer";
import MusicPlayerInReact from "./MusicPlayerInReact";

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
        <><p>Music Library</p>
        {/*<ol>
          {songs.map((index, key) =>
            <li key={key}>{index}</li>
          )}
        </ol>*/}
        <MusicPlayerInReact/>
      </>
    );
}