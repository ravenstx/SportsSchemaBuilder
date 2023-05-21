import React, { useState, useEffect } from 'react';
import { API_URL } from '../util/Urls';
import '../css/Scheme.css';
import '../css/CalendarEditContainer.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDumbbell } from '@fortawesome/free-solid-svg-icons';
import { faPersonBiking } from '@fortawesome/free-solid-svg-icons';
import { faPersonRunning } from '@fortawesome/free-solid-svg-icons';
import { faPersonSwimming } from '@fortawesome/free-solid-svg-icons';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { faMinus } from '@fortawesome/free-solid-svg-icons';

const days = [
  'Sunday',
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
];
const months = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December',
];

const categorys = ['Cycling', 'Running', 'Strength', 'Swimming'];

function CalendarEditContainer(props) {
  const [category, setCategory] = useState(null);
  const [hours, setHours] = useState('');
  const [minutes, setMinutes] = useState('');
  const [description, setDescription] = useState('');
  const [strengthExercises, setStrengthExercises] = useState([
    { exerciseName: '', sets: '', reps: '' },
  ]);

  useEffect(() => {}, []);

  const handleMinutesChange = (e) => {
    if (isNaN(+e.target.value)) {
      return;
    }
    setMinutes(e.target.value);
  };
  const handleHoursChange = (e) => {
    if (isNaN(+e.target.value)) {
      return;
    }
    setHours(e.target.value);
  };

  const handleAddButtonClick = () => {
    if (
      strengthExercises[strengthExercises.length - 1].exerciseName == '' ||
      strengthExercises[strengthExercises.length - 1].sets == '' ||
      strengthExercises[strengthExercises.length - 1].reps == ''
    ) {
      return;
    }
    setStrengthExercises([
      ...strengthExercises,
      { exerciseName: '', sets: '', reps: '' },
    ]);
  };
  const handleStrengthExercisesInputChange = (e, index, key) => {
    if (key != 'exerciseName' && isNaN(+e.target.value)) {
      return;
    }
    let copy = [...strengthExercises];

    copy[index][key] = e.target.value;

    setStrengthExercises(copy);
  };
  const handleDeleteButtonClick = (index) => {
    if (strengthExercises.length <= 1) {
      return;
    }
    let copy = [...strengthExercises];

    copy.splice(index, 1);

    setStrengthExercises(copy);
  };

  const UploadStrengthExerciseWorkout = async () => {
    let copy = [...strengthExercises];

    if (
      strengthExercises[strengthExercises.length - 1].exerciseName == '' ||
      strengthExercises[strengthExercises.length - 1].sets == '' ||
      strengthExercises[strengthExercises.length - 1].reps == ''
    ) {
      copy.splice(strengthExercises.length - 1, 1);
    }

    if (copy.length == 0) {
      return;
    }

    try {
      const res = await fetch(API_URL + '/api/Calendar/Upload', {
        method: 'POST',
        credentials: 'include',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },

        body: JSON.stringify({
          category: category,
          date: new Date(
            props.date.getTime() - props.date.getTimezoneOffset() * 60000
          )
            .toISOString()
            .split('T')[0],
          exerciseList: copy,
        }),
      });
      if (res.ok) {
        const data = await res.json();
        props.setReloadUserData(true);
        props.setInEditingMode(false);
        console.log(data);
      }
    } catch (error) {
      console.log(error);
    }
  };
  const handleUpload = async () => {
    if (description === '' && hours === '' && minutes === '') {
      return;
    }

    try {
      const res = await fetch(API_URL + '/api/Calendar/Upload', {
        method: 'POST',
        credentials: 'include',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },

        body: JSON.stringify({
          category: category,
          date: new Date(
            props.date.getTime() - props.date.getTimezoneOffset() * 60000
          )
            .toISOString()
            .split('T')[0],
          DurationHours: parseInt(hours),
          DurationMinutes: parseInt(minutes),
          description: description,
        }),
      });
      if (res.ok) {
        const data = await res.json();
        props.setReloadUserData(true);
        props.setInEditingMode(false);
        console.log(data);
      }
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <>
      <div className="overlay" onClick={(e) => props.setInEditingMode(false)}>
        <div
          className="editing-box"
          onClick={(e) => e.stopPropagation()}
          style={category == null ? { height: '180px' } : null}
        >
          <div className="editing-box-top">
            <h1 className="modal-title">
              {days[props.date.getDay()]}, {props.date.getDate()}{' '}
              {months[props.date.getMonth()]}
            </h1>
            <button onClick={(e) => props.setInEditingMode(false)}>
              <FontAwesomeIcon icon={faXmark} color="gray" size="2x" />
            </button>
          </div>
          <div className="edit-body">
            <div className="activity-buttons">
              <p>Pick a Category:</p>
              <div className="activity-buttons-container">
                <button
                  className={category == 0 ? 'selected-button' : ''}
                  onClick={(e) => setCategory(0)}
                >
                  <FontAwesomeIcon
                    icon={faPersonBiking}
                    color="black"
                    size="2x"
                  />
                </button>
                <button
                  className={category == 1 ? 'selected-button' : ''}
                  onClick={(e) => setCategory(1)}
                >
                  <FontAwesomeIcon
                    icon={faPersonRunning}
                    color="black"
                    size="2x"
                  />
                </button>
                <button
                  className={category == 2 ? 'selected-button' : ''}
                  onClick={(e) => setCategory(2)}
                >
                  <FontAwesomeIcon icon={faDumbbell} color="black" size="2x" />
                </button>
                <button
                  className={category == 3 ? 'selected-button' : ''}
                  onClick={(e) => setCategory(3)}
                >
                  <FontAwesomeIcon
                    icon={faPersonSwimming}
                    color="black"
                    size="2x"
                  />
                </button>
              </div>
            </div>
            {category === null ? null : (
              <div className="input-group">
                {category === 2 ? (
                  <>
                    <div className="strenght-exercises-container">
                      {strengthExercises.map((obj, index) => (
                        <div key={index} className="strenght-exercises">
                          <div className="input-container">
                            <div className="exercise-input">
                              <input
                                type="text"
                                placeholder="exercise"
                                value={obj.exerciseName}
                                onChange={(e) =>
                                  handleStrengthExercisesInputChange(
                                    e,
                                    index,
                                    'exerciseName'
                                  )
                                }
                              ></input>
                            </div>
                            <div className="sets">
                              <input
                                type="text"
                                maxLength={3}
                                placeholder="sets"
                                value={obj.sets}
                                onChange={(e) =>
                                  handleStrengthExercisesInputChange(
                                    e,
                                    index,
                                    'sets'
                                  )
                                }
                              ></input>
                            </div>
                            <FontAwesomeIcon
                              icon={faX}
                              color="gray"
                              size="1x"
                            />
                            <div className="reps">
                              <input
                                type="text"
                                maxLength={3}
                                placeholder="reps"
                                value={obj.reps}
                                onChange={(e) =>
                                  handleStrengthExercisesInputChange(
                                    e,
                                    index,
                                    'reps'
                                  )
                                }
                              ></input>
                            </div>
                          </div>

                          <div
                            className="delete-button"
                            onClick={() => handleDeleteButtonClick(index)}
                          >
                            <FontAwesomeIcon
                              icon={faMinus}
                              color="black"
                              size="1x"
                            />
                          </div>
                        </div>
                      ))}

                      <button
                        type="button"
                        className="add-button"
                        onClick={handleAddButtonClick}
                      >
                        <FontAwesomeIcon
                          icon={faPlus}
                          color="white"
                          size="1x"
                        />
                      </button>
                    </div>

                    <div
                      className="save-button"
                      onClick={(e) => UploadStrengthExerciseWorkout()}
                    >
                      <button type="button" className="btn btn-dark">
                        Save
                      </button>
                    </div>
                  </>
                ) : (
                  <>
                    <div className="duration-picker-container">
                      <div className="duration-picker">
                        <div className="hours">
                          <p>Hours</p>
                          <input
                            value={hours}
                            placeholder="h"
                            maxLength="1"
                            style={{ textAlign: 'center' }}
                            onChange={(e) => handleHoursChange(e)}
                          ></input>
                        </div>
                        <p className="middle">:</p>
                        <div className="minutes">
                          <p>Minutes</p>
                          <input
                            value={minutes}
                            placeholder="mm"
                            maxLength="2"
                            onChange={(e) => handleMinutesChange(e)}
                          ></input>
                        </div>
                      </div>
                    </div>

                    <div className="description">
                      <label htmlFor="exampleFormControlTextarea1">
                        Training description:
                      </label>
                      <textarea
                        style={{}}
                        className="form-control"
                        id="exampleFormControlTextarea1"
                        rows="2"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                      ></textarea>
                    </div>
                    <div
                      className="save-button"
                      onClick={(e) => handleUpload()}
                    >
                      <button type="button" className="btn btn-dark">
                        Save
                      </button>
                    </div>
                  </>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </>
  );
}

export default CalendarEditContainer;
