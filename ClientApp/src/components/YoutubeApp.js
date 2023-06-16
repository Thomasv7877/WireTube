import React, { useState, useEffect } from "react";
import '../custom.css'

// search and download youtube videos
export function YoutubeApp(){
    const [searchTerm, setSearchTerm] = useState('');
    const [searchTermAlt, setSearchTermAlt] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const apiKey = process.env.react_app_yt_api_key;
    const apiUrl = `https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=10&q=${searchTerm}&type=video&key=${apiKey}`;
    const [searchResults, setSearchResults] = useState([]);
    const [searchToggle, setSearchToggle] = useState(false);
    
    // untoggled: search videos using the youtube API, needs a token
    const handleSearch = async () => {
        try {
          setIsLoading(true);
          const response = await fetch(apiUrl);
          const data = await response.json();
          for(var i = 0; i < data.items.length; i++){
            var detailUrl = `https://youtube.googleapis.com/youtube/v3/videos?part=snippet,statistics&id=${data.items[i].id.videoId}&key=${apiKey}`;
            const subResponse = await fetch(detailUrl);
            const subData = await subResponse.json();
            data.items[i]['views'] = subData.items[0].statistics.viewCount;
            data.items[i]['downloading'] = false;
            data.items[i]['dlProgress'] = 0;
          }
          setSearchResults(data.items);
          setIsLoading(false);
        } catch (error) {
          console.error('Error:', error);
        }
      };

    // toggled: send url of regular yoiutube search to backend, will scrape the raw html page and return results
    // REM: will break if youtube changes page formatting
    const handleSearchNoAPI = async () => {
      try {
        setIsLoading(true);
        const yturl = `https://www.youtube.com/results?search_query=${searchTermAlt}`;
        const options = {
          method: 'POST',
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify(yturl)
        }
        const response = await fetch("/ytApi/search", options);
        const data = await response.json();
        setSearchResults(data);
        setIsLoading(false);
      } catch (e){
        console.error('Error:', e);
      };
    };

    // send video id to backend, wil audio will be ripped using youtube-dl or yt-dlp
    const handleDownload = async (vidUrl, vidTitle) => {
      try {
        const response = await fetch("/ytApi/dl", {
          method: 'POST',
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify({
            url: vidUrl, 
            title: vidTitle})
            });
        const data = await response.json();
      } catch (error) {
        console.error('Error:', error);
      }
    }
    const handleKeyDown = (event) => {
      if(event.key === 'Enter'){
        handleSearch();
      }
    }
    const handleKeyDownAlt = (event) => {
      if(event.key === 'Enter'){
        handleSearchNoAPI();
      }
    }

    // socket for download progress
    useEffect(() => {
      const eventSource = new EventSource('/ytApi/dlprogress');
      eventSource.addEventListener('update', (event) => {
        const eventData = JSON.parse(event.data);
        console.log("got something.. title = " + eventData.Title + " progress = " + eventData.Progress);
        // nieuwe array maken, wegens ondiepe vgl anders geen rerender getriggered
        var newSearchResults = [...searchResults];
        newSearchResults.filter(obj => obj.snippet.title === eventData.Title).forEach(obj => {
          obj.downloading = true;
          obj.progress = eventData.Progress;
        });
        setSearchResults(newSearchResults);
      });
      eventSource.addEventListener('error', (event) => {
        console.log("something went wrong.." + event);
      });
      /*return () => { // niet gebruiken anders gegarandeerd elke render nieuwe connectie
        // Cleanup function to close the SSE connection
        eventSource.close();
      };*/
    }, [searchResults]);

  return (
    <div className="yt-search-wrapper">
      <p id="search-title">Find something on youtube</p>
      {searchToggle ?
        <input id="searchYt" className="col-6" type="text" value={searchTermAlt} onChange={e => setSearchTermAlt(e.target.value)} onKeyDown={handleKeyDownAlt} placeholder="Search songs... without API token" />
        :
        <input id="searchYt" className="col-6" type="text" value={searchTerm} onChange={e => setSearchTerm(e.target.value)} onKeyDown={handleKeyDown} placeholder="Search songs..." />
      }
      <div id="searchToggle" className="form-check form-switch">
        <input className="form-check-input" type="checkbox" role="switch" id="flexSwitchCheckDefault" onClick={() => setSearchToggle(!searchToggle)} />
      </div>
      {!isLoading ? (
        <ul id="ytResults">
          {searchResults.map(result => {
            var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
            return (
              <li key={result.id.videoId} className="clearfix row">
                <a href={vidLink} className="col-md-2 col-6"><img src={result.snippet.thumbnails.medium.url} alt={result.snippet.title} className="img-fluid"></img></a>
                <div className="col-md-7 col-6"><h5>{result.snippet.title}</h5>
                  <p>{result.snippet.channelTitle} - {formatViews(result.views)} views <button type="button" className="btn" onClick={() => handleDownload(vidLink, result.snippet.title)}><i className={formatButton(result.downloading, result.progress)}></i></button></p>
                  <div className="progress col-6" id="progressBar" style={{ visibility: result.downloading ? 'visible' : 'hidden' }}>
                    <div className={`progress-bar ${result.progress < 100 ? "progress-bar-striped progress-bar-animated" : ""}`} role="progressbar" aria-valuenow={result.progress} aria-valuemin="0" aria-valuemax="100" style={{ width: result.progress + "%" }}></div>
                  </div>
                </div>
              </li>
            )
          })}
        </ul>) : (<div className="spinner-grow text-primary m-5" role="status">
          <span className="sr-only">test</span>
        </div>)}
    </div>
  );
}

// helper function for view counts
function formatViews(views){
  if(views > 1000000){
    return parseInt(Math.round(views / 1000000)) + " mln.";
  } else if (views > 1000){
    return parseInt(Math.round(views / 1000)) + "k";
  } else {
    return views;
  }
}
// helper function: show spinner when download active, checkmark when done/100%
function formatButton(downloading, progress){
  if(downloading && progress === 100){
    return "icon bi-check-lg";
  } else if(downloading){
    return "spinner-border spinner-border-sm";
  } else {
    return "icon bi-download";
  }
}