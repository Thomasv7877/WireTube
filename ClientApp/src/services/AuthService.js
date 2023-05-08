import { handleResponse } from "../helpers/handle-response";
//import React, { useEffect, useState } from "react";

const getCurrentUser = () => {
    return JSON.parse(localStorage.getItem("currentUser"));
  };

//   const login = async (username, password) => {
//     return await fetch("/Users/authenticate", {
//         method: 'POST',
//         headers: {'Content-Type': 'application/json'},
//         body: JSON.stringify({username, password})
//    }
//    ).then((response) => {
//     const data = JSON.parse(response.text());
//     console.log("Post OK! - data:" + data);
//     if (response.data.username) {
//       localStorage.setItem("user", JSON.stringify(response.data));
//     }
//     return response.data;
//   });
// }

function login(username, password) {
  const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
  };

  return fetch("/Users/authenticate", requestOptions)
      .then(handleResponse)
      .then(user => {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          console.log(user.username)
          localStorage.setItem('currentUser', JSON.stringify(user));
          //getCurrentUser = user;

          return user;
      });
}
  
  // const AuthService = {
  //   login,
  //   getCurrentUser
  // }
  
  //export default AuthService;
  export const AuthService = {
    login,
    getCurrentUser
  }