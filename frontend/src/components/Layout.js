import React, { Component } from 'react';
import NavBar from './NavBar';
import '../css/Layout.css';

function Layout(props) {
  return (
    <div className="dashboard">
      <NavBar />
      {props.children}
    </div>
  );
}

export default Layout;
