import React, { useState, useEffect } from 'react';
import { API_URL } from '../util/Urls';

import Charts from './Chart';
import '../css/Analysis.css';

function AnalysisDataBoard(props) {
  const secondsToHoursMinutesAndSeconds = (seconds) => {
    return new Date(seconds * 1000).toISOString().slice(11, 19);
  };

  return (
    <>
      {props.selectedFileIndex == null ? null : (
        <div className="data-board">
          <div className="top-info">
            <div>
              <p>total distance</p>
              <h1>
                {parseFloat(
                  props.fitFileData['activity']['sessions'][0][
                    'total_distance'
                  ].toFixed(2)
                )}{' '}
                km
              </h1>
            </div>

            <div>
              <p>total time</p>
              <h1>
                {secondsToHoursMinutesAndSeconds(
                  props.fitFileData['activity']['total_timer_time']
                )}
              </h1>
            </div>

            <div>
              <p>average speed</p>
              <h1>
                {parseFloat(
                  props.fitFileData['activity']['sessions'][0][
                    'avg_speed'
                  ].toFixed(2)
                )}{' '}
                km/h
              </h1>
            </div>

            <div>
              <p>total ascent</p>
              <h1>
                {parseFloat(
                  1000 *
                    props.fitFileData['activity']['sessions'][0][
                      'total_ascent'
                    ].toFixed(2)
                )}{' '}
                m
              </h1>
            </div>
            <div>
              {props.fitFileData['activity']['sessions'][0]['avg_heart_rate'] !=
              null ? (
                <>
                  <p>average heart rate</p>
                  <h1>
                    {
                      props.fitFileData['activity']['sessions'][0][
                        'avg_heart_rate'
                      ]
                    }{' '}
                    bpm
                  </h1>
                </>
              ) : null}
            </div>
          </div>
          <div className="charts-box">
            <div className="charts">
              <Charts
                data={props.fitFileData['activity']['sessions'][0]['laps']}
                avg_heart_rate={
                  props.fitFileData['activity']['sessions'][0]['avg_heart_rate']
                }
              />
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default AnalysisDataBoard;
