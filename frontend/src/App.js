import './App.css';
import Login from './components/Login';
import ProtectedRoute from './util/ProtectedRoute';
import Home from './components/Home';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { useState } from 'react';

import Layout from './components/Layout';

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="login" element={<Login />} />
        <Route path="/" element={<ProtectedRoute />}>
          <Route
            path=""
            element={
              <Layout>
                <Home />
              </Layout>
            }
          />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
