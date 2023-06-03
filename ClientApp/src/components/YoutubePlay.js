import React from "react";
//import MusicPlayer from "./MusicPlayer";
import MusicPlayerInReact from "./MusicPlayerInReact";

// psotman test: [GET]https://localhost:7066/YtApi

export function YoutubePlay(){
    //var songs = [];
    //const [songs, setSongs] = useState([]);

    // fetching songs handled by MusicPlayerInReact component
    /*useEffect(() => {
      const fetchSongs = async () => {
        const response = await fetch('/ytApi');
        const data = await response.json();
        setSongs(data);
    };
    fetchSongs();
    }, [])*/
    
      
    return(
        <><p id="lib-title">Music Library</p>
        {/*<ol>
          {songs.map((index, key) =>
            <li key={key}>{index}</li>
          )}
        </ol>*/}
        <MusicPlayerInReact/>
      </>
    );
}