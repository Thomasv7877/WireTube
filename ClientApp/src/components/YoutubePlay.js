import React, { useState } from "react";

// psotman test: [GET]https://localhost:7066/YtApi

export function YoutubePlay(){
    const songs = [];

    fetch("/YtApi")
    .then(response => console.log(response))
    .catch(error => {
      console.error('Error fetching data:', error);
    });

      
    return(
        <><p>Playlist</p>
        <ol>
          {songs.map(song =>
            <li>{song}</li>
          )}
        </ol>
      </>
    );
}