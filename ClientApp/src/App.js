import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';
import { PrivateRoute } from './components/PrivateRoute';

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Routes>
        {AppRoutes.map((route, index) => {
            const { element, requireAuth, ...rest } = route;
            return <Route key={index} {...rest} element={requireAuth ? <PrivateRoute {...rest} element={element} /> : element} />; //  path={path} niet nodig, reeds via ...rest te gebruiken
          })}
        </Routes>
      </Layout>
    );
  }
}
