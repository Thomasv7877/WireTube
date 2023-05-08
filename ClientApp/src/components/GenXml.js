import React, { useEffect, useState } from "react";
//import DateTimePicker from 'react-datetime-picker'; 
//import DatePicker from "react-datepicker";
//import "react-datepicker/dist/react-datepicker.css";
import {AuthService}  from '../services/AuthService';
import { handleResponse } from "../helpers/handle-response";
import { authHeader } from "../services/AuthHeader";

export function GenXml(){
    const currentDate = new Date();
    const timezoneOffset = currentDate.getTimezoneOffset() * 60000; // Offset in milliseconds
    const adjustedDate = new Date(currentDate.getTime() - timezoneOffset);
    const formattedAdjustedDate = adjustedDate.toISOString().slice(0, 16);
    
    const [startDate, setStartDate] = useState(formattedAdjustedDate);
    const [endDate, setEndDate] = useState(formattedAdjustedDate);
    const [service, setService] = useState("");
    const[serviceState, setServiceState] = useState({ services: [], loading: true});
    //console.log("Eerste service: ") + services;
    //const [formInput, setFormInput] = useState({})

    /*var content = serviceState.services.map(service => // niet gebruikt, inline  xml gen
        <option key={service} value={service}>{service}</option>
        )*/
    
      useEffect(() => {
        populateServices();
      }, [])

      const handleSubmit = (event) => {
        event.preventDefault();
        //alert(`Submitted Form!\nStart: ${startDate}\nEnd: ${endDate}\nService: ${service}`);
        postForm();
      }

      const currentUser = AuthService.getCurrentUser();
      //AuthService.getCurrentUser.then(handleResponse)
      console.log("Current user is: " + currentUser.username);

    return( 
        <>
            <p id="xmlTitel">Maak trigger XML</p>
            <form className="xmlForm" onSubmit={handleSubmit}>
            <select name="services" id="services" onChange={e => setService(e.target.value)}>
                <option disabled selected>--Kies een service--</option>
                {/*content*/}
                {serviceState.services.map(service => 
                    <option key={service} value={service}>{service}</option>
                    )}
            </select>
            <div class="col-4"><label for="start">Start</label>
            <input type="datetime-local" id="start" onChange={e => setStartDate(e.target.value)} value={startDate}/></div>
            <div class="col-4"><label for="einde">Einde</label>
            <input type="datetime-local" id="einde" onChange={e => setEndDate(e.target.value)} value={endDate}/></div>
            <button className="btn btn-primary" type="submit">Genereer</button>
            </form>
            <button className="btn btn-secondary" style={{margin: '10px'}} onClick={postTest}>lukt dit</button>
            <button className="btn btn-secondary" style={{margin: '10px'}} onClick={getAll}>Fetch users</button>
        </>
    );

    async function populateServices() {
        const response = await fetch('trigger');
        const data = await response.json();
        setServiceState({services: data, loading: false});
      }
    async function postForm(){
        await fetch("/trigger", {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({startDate, endDate, service})
       }
       );
    }
    async function postTest(){
      await fetch("/test", {
          method: 'POST',
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify(service)
     }
     );
  }
  async function getAll() {
    const requestOptions = { method: 'GET', headers: authHeader() };
    var response = await fetch("/Users", requestOptions).then(handleResponse);
    //var data = JSON.parse(response);
    console.log(response[0].username);
}
}

