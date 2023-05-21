import React, { useState, useEffect } from 'react';
import { Route, useNavigate } from 'react-router-dom';
import { API_URL } from '../util/Urls';
import '../css/Home.css';
import Analysis from './Analysis';
import Scheme from './Scheme';

function Home() {
  const navigate = useNavigate();
  const [analyseSlideSelected, setAnalyseSlideSelected] = useState(false);
  const [schemaSlideSelected, setSchemaSlideSelected] = useState(true);

  return (
    <div className="dashboard-container">
      <div className="accordion-container">
        <div
          className={analyseSlideSelected ? 'dropdown selected' : 'dropdown'}
          onClick={() => setAnalyseSlideSelected(!analyseSlideSelected)}
        >
          <div
            className={
              analyseSlideSelected ? 'trapezoid selected' : 'trapezoid'
            }
          ></div>
          {analyseSlideSelected ? (
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="28"
              height="28"
              fill="currentColor"
              className="bi bi-chevron-compact-up"
              viewBox="0 0 16 16"
            >
              <path
                fillRule="evenodd"
                d="M7.776 5.553a.5.5 0 0 1 .448 0l6 3a.5.5 0 1 1-.448.894L8 6.56 2.224 9.447a.5.5 0 1 1-.448-.894l6-3z"
              />
            </svg>
          ) : (
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="28"
              height="28"
              fill="currentColor"
              className="bi bi-chevron-compact-down"
              viewBox="0 0 16 16"
            >
              <path
                fillRule="evenodd"
                d="M1.553 6.776a.5.5 0 0 1 .67-.223L8 9.44l5.776-2.888a.5.5 0 1 1 .448.894l-6 3a.5.5 0 0 1-.448 0l-6-3a.5.5 0 0 1-.223-.67z"
              />
            </svg>
          )}

          <p>ANALYSIS</p>
        </div>
        {analyseSlideSelected ? <Analysis /> : null}
        <div
          className={schemaSlideSelected ? 'dropdown selected' : 'dropdown'}
          onClick={() => setSchemaSlideSelected(!schemaSlideSelected)}
        >
          <div
            className={schemaSlideSelected ? 'trapezoid selected' : 'trapezoid'}
          ></div>
          {schemaSlideSelected ? (
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="28"
              height="28"
              fill="currentColor"
              className="bi bi-chevron-compact-up"
              viewBox="0 0 16 16"
            >
              <path
                fillRule="evenodd"
                d="M7.776 5.553a.5.5 0 0 1 .448 0l6 3a.5.5 0 1 1-.448.894L8 6.56 2.224 9.447a.5.5 0 1 1-.448-.894l6-3z"
              />
            </svg>
          ) : (
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="28"
              height="28"
              fill="currentColor"
              className="bi bi-chevron-compact-down"
              viewBox="0 0 16 16"
            >
              <path
                fillRule="evenodd"
                d="M1.553 6.776a.5.5 0 0 1 .67-.223L8 9.44l5.776-2.888a.5.5 0 1 1 .448.894l-6 3a.5.5 0 0 1-.448 0l-6-3a.5.5 0 0 1-.223-.67z"
              />
            </svg>
          )}

          <p>SCHEME</p>
        </div>
        {schemaSlideSelected ? <Scheme /> : null}
      </div>
    </div>
  );
}

export default Home;
