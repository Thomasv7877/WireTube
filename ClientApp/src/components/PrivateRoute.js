import React from 'react';
import { Navigate } from 'react-router-dom';

import {AuthService}  from '../services/AuthService';

export function PrivateRoute({element, path}) {
    const currentUser = AuthService.getCurrentUser();
    //const { element } = this.props;
    //console.log("Going to" + path);
    
    return currentUser ? element : <Navigate replace to={"/Login"} state={{targetRoute: path}}/>;
}