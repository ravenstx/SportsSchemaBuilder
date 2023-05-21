import React, { useState, useEffect } from 'react';
import { Route, useNavigate, Navigate } from 'react-router-dom';
import { API_URL } from '../util/Urls';
import '../css/Login.css';

function Login(props) {
  const [registerState, setregisterState] = useState(false);
  const [loginData, setLoginData] = useState({
    name: '',
    password: '',
    passwordRepeat: '',
  });
  const [loggedIn, setLoggedIn] = useState(false);
  const [loginFailed, setLoginFailed] = useState(false);
  const navigate = useNavigate();

  function update(value, fieldName, obj) {
    setLoginData({ ...obj, [fieldName]: value });
    setLoginFailed(false);
  }

  async function login(e) {
    try {
      const res = await fetch(API_URL + '/api/UserAuth/Login', {
        method: 'POST',
        credentials: 'include',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          accept: '*/*',
        },

        body: JSON.stringify({
          name: loginData.name,
          Password: loginData.password,
        }),
      });
      if (!res.ok) {
        setLoginFailed(true);
        throw new Error('Bad Response');
      }
      if (res.ok) {
        const data = await res.json();
        const token = data['token'];
        localStorage.clear();
        localStorage.setItem('user-token', token);
        console.log(localStorage['user-token']);
        setLoggedIn(true);
      }
    } catch (error) {
      console.log(error);
    }
  }

  async function register(e) {
    console.log(loginData);
    if (
      loginData.password == '' ||
      loginData.password != loginData.passwordRepeat
    ) {
      return;
    }
    try {
      const res = await fetch(API_URL + '/api/UserAuth/register', {
        method: 'POST',
        credentials: 'include',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          accept: '*/*',
        },

        body: JSON.stringify({
          name: loginData.name,
          Password: loginData.password,
        }),
      });
      if (!res.ok) {
        setLoginFailed(true);
        throw new Error('Bad Response');
      }
      if (res.ok) {
        setregisterState(false);
      }
    } catch (error) {
      console.log(error);
    }
  }

  return (
    <>
      <img src={require('../util/athena.png')} />
      <div className="login-container">
        <div className="LoginRegister">
          <div
            className="btn-group"
            role="group"
            aria-label="Basic outlined example"
          >
            <button
              type="button"
              className={
                registerState
                  ? 'btn btn-outline-primary'
                  : 'btn btn-outline-primary active'
              }
              onClick={(e) => setregisterState(false)}
            >
              Login
            </button>
            <button
              type="button"
              className={
                registerState
                  ? 'btn btn-outline-primary active'
                  : 'btn btn-outline-primary'
              }
              onClick={(e) => setregisterState(true)}
            >
              Register
            </button>
          </div>

          <div className="floating-label-content">
            <input
              className="floating-input"
              type="text"
              name="name"
              placeholder=" "
              value={loginData.name}
              onChange={(evt) => update(evt.target.value, 'name', loginData)}
            />

            <label
              className={
                loginData.name === ''
                  ? 'empty-floating-label'
                  : 'filled-floating-label'
              }
            >
              Username
            </label>
          </div>
          <div className="floating-label-content">
            <input
              className="floating-input"
              type="password"
              name="password"
              placeholder=" "
              value={loginData.password}
              onChange={(evt) =>
                update(evt.target.value, 'password', loginData)
              }
            />
            <label
              className={
                loginData.password === ''
                  ? 'empty-floating-label'
                  : 'filled-floating-label'
              }
            >
              Password
            </label>
          </div>

          {registerState ? (
            <div className="floating-label-content">
              <input
                className="floating-input"
                type="password"
                name="password"
                placeholder=" "
                value={loginData.passwordRepeat}
                onChange={(evt) =>
                  update(evt.target.value, 'passwordRepeat', loginData)
                }
              />
              <label
                className={
                  loginData.passwordRepeat === ''
                    ? 'empty-floating-label'
                    : 'filled-floating-label'
                }
              >
                Repeat password
              </label>
            </div>
          ) : null}

          {registerState ? (
            <button
              className="submit btn btn-outline-primary btn-block mb-4"
              value="Submit"
              onClick={register}
            >
              Register
            </button>
          ) : (
            <button
              className="submit btn btn-outline-primary btn-block mb-4"
              value="Submit"
              onClick={login}
            >
              Login
            </button>
          )}

          <div>
            {loginFailed ? (
              <h2 style={{ color: 'red' }}>name or password is wrong!</h2>
            ) : null}
          </div>

          <div>{loggedIn ? <Navigate to="/" /> : null}</div>
        </div>
      </div>
    </>
  );
}

export default Login;
