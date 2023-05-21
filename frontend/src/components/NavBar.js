import React, { useState, useEffect } from 'react';
import { Route, useNavigate, Navigate } from 'react-router-dom';
import { API_URL } from '../util/Urls';
import '../css/NavBar.css';

function NavBar(props) {
  const navigate = useNavigate();
  const [usersArray, setUsersArray] = useState([]);

  const loadData = async () => {
    try {
      const res = await fetch(API_URL + '/api/UserAuth/User', {
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      const data = await res.json();
      console.log(data);
      setUsersArray(data);
    } catch (error) {
      console.log(error);
      console.log('call refresh');
      refreshjwt();
    }
  };

  const refreshjwt = async () => {
    try {
      const res = await fetch(API_URL + '/api/UserAuth/Refresh', {
        method: 'POST',
        mode: 'cors',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      if (!res.ok) {
        localStorage.clear();
        return navigate('/login');
      }
      if (res.ok) {
        const data = await res.json();
        const token = data['newJWT'];
        console.log(token);
        localStorage.setItem('user-token', token);
        window.location.reload();
      }
    } catch (error) {
      console.log(error);
    }
  };

  function logout() {
    localStorage.clear();
    navigate('/login');
  }

  useEffect(() => {
    loadData();
  }, []);
  return (
    <ul className="nav justify-content-end navbar sticky-top">
      <div className="nav-trapezoid"></div>
      <h1 className="logo">SSB</h1>

      <li className="profile-icon">
        {usersArray.name ? (
          <div className="profile-icon-char">
            {usersArray.name.charAt(0).toUpperCase()}
          </div>
        ) : null}
      </li>
      <li className="nav-item">
        <a
          style={{ fontWeight: '600' }}
          className="nav-link nav-logout"
          onClick={logout}
        >
          Sign out
        </a>
      </li>

      <div className="nav-trapezoid-right"></div>
    </ul>
  );
}

export default NavBar;
