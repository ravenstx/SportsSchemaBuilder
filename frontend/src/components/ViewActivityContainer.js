import React, { useState, useEffect } from 'react';
import { API_URL } from '../util/Urls';
import '../css/Scheme.css';
import '../css/ViewActivityContainer.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDumbbell, faPen } from '@fortawesome/free-solid-svg-icons';
import { faPersonBiking } from '@fortawesome/free-solid-svg-icons';
import { faPersonRunning } from '@fortawesome/free-solid-svg-icons';
import { faPersonSwimming } from '@fortawesome/free-solid-svg-icons';
import { faX } from '@fortawesome/free-solid-svg-icons';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { faMinus } from '@fortawesome/free-solid-svg-icons';
import { faTrashCan } from '@fortawesome/free-solid-svg-icons';
import { faPencil } from '@fortawesome/free-solid-svg-icons';

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

const categorys = ['Cycling', 'Running', 'Strength Training', 'Swimming'];

const activityIcons = [
  faPersonBiking,
  faPersonRunning,
  faDumbbell,
  faPersonSwimming,
];

const activityColor = ['#092b63', '#28a745', '#5e0000', '#72e0ff'];

function ViewActivityContainer(props) {
  const [inEditingMode, setInEditingMode] = useState(false);
  const [deleteIdList, setDeleteIdList] = useState([]);
  const [dataCopy, setDataCopy] = useState(
    JSON.parse(JSON.stringify(props.data))
  );

  useEffect(() => {}, []);

  const deleteActivity = async (id) => {
    try {
      const res = await fetch(API_URL + `/api/Calendar/DeleteActivity/${id}`, {
        method: 'DELETE',
        credentials: 'include',
        mode: 'cors',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      if (res.ok) {
        props.setReloadUserData(true);
        props.setInViewActivityMode(false);
        const data = await res.json();
        console.log(data);
      }
    } catch (error) {
      console.log(error);
    }
  };

  const updateActivity = async () => {
    let payload = { id: props.data.id };
    if (props.data.category === 2) {
      let list = [];
      dataCopy.exerciseList.forEach((element) => {
        if (
          !element.id &&
          element.exerciseName != '' &&
          element.sets != '' &&
          element.reps != ''
        ) {
          list.push(element);
        } else {
          for (let i of props.data.exerciseList) {
            if (i.id == element.id) {
              if (
                i.exerciseName != element.exerciseName ||
                i.sets != parseInt(element.sets) ||
                i.reps != parseInt(element.reps)
              ) {
                list.push(element);
              }
            }
          }
        }
      });
      payload.exerciseList = list;
      payload.deleteIdList = deleteIdList;
    } else {
      payload.durationHours = parseInt(dataCopy.durationHours);
      payload.durationMinutes = parseInt(dataCopy.durationMinutes);
      payload.description = dataCopy.description;
    }

    console.log(JSON.stringify(payload));
    try {
      const res = await fetch(API_URL + `/api/Calendar/UpdateActivity`, {
        method: 'PUT',
        credentials: 'include',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
        body: JSON.stringify(payload),
      });
      if (res.ok) {
        props.setReloadUserData(true);
        props.setInViewActivityMode(false);
      }
    } catch (error) {
      console.log(error);
    }
  };

  const handleEditButtonClick = () => {
    setInEditingMode(true);
  };
  const handleCancelClick = () => {
    setInEditingMode(false);
    setDeleteIdList([]);
    setDataCopy(JSON.parse(JSON.stringify(props.data)));
  };

  const handleHoursChange = (e) => {
    setDataCopy({ ...dataCopy, durationHours: e.target.value });
  };
  const handleMinutesChange = (e) => {
    setDataCopy({ ...dataCopy, durationMinutes: e.target.value });
  };
  const handleAddButtonClick = () => {
    if (dataCopy.exerciseList.length == 0) {
      return setDataCopy({
        ...dataCopy,
        exerciseList: [
          ...dataCopy.exerciseList,
          { exerciseName: '', sets: '', reps: '' },
        ],
      });
    }
    if (
      dataCopy.exerciseList[dataCopy.exerciseList.length - 1].exerciseName ==
        '' ||
      dataCopy.exerciseList[dataCopy.exerciseList.length - 1].sets == '' ||
      dataCopy.exerciseList[dataCopy.exerciseList.length - 1].reps == ''
    ) {
      return;
    }
    setDataCopy({
      ...dataCopy,
      exerciseList: [
        ...dataCopy.exerciseList,
        { exerciseName: '', sets: '', reps: '' },
      ],
    });
  };

  const handleDeleteButtonClick = (index) => {
    if (dataCopy.exerciseList.length <= 1) {
      return;
    }
    if (dataCopy.exerciseList[index].id) {
      setDeleteIdList((deleteIdList) => [
        ...deleteIdList,
        dataCopy.exerciseList[index].id,
      ]);
    }

    let copy = [...dataCopy.exerciseList];
    copy.splice(index, 1);
    setDataCopy({
      ...dataCopy,
      exerciseList: copy,
    });
  };

  const handleStrengthExercisesInputChange = (e, index, key) => {
    if (key != 'exerciseName' && isNaN(+e.target.value)) {
      return;
    }
    let copy = [...dataCopy.exerciseList];

    copy[index][key] = e.target.value;

    setDataCopy({
      ...dataCopy,
      exerciseList: copy,
    });
  };
  return (
    <>
      <div
        className="overlay"
        onClick={(e) => props.setInViewActivityMode(false)}
      >
        <div
          className={props.data.category == 2 ? 'editing-box' : 'editing-box'}
          style={
            props.data.category == 2
              ? { height: 'fit-content' }
              : { height: 'fit-content' }
          }
          onClick={(e) => e.stopPropagation()}
        >
          <div
            className="editing-box-top"
            style={{
              backgroundColor: `${activityColor[props.data.category]}`,
            }}
          >
            <h1 className="modal-title top-title">
              <FontAwesomeIcon
                icon={activityIcons[props.data.category]}
                color="white"
                size="1x"
              />
              {days[new Date(props.data.date).getDay()]},{' '}
              {new Date(props.data.date).getDate()}{' '}
              {months[new Date(props.data.date).getMonth()]}
            </h1>
            <button onClick={(e) => props.setInViewActivityMode(false)}>
              <FontAwesomeIcon icon={faXmark} color="white" size="2x" />
            </button>
          </div>

          <div className="edit-body" style={{ paddingTop: '0' }}>
            <div className="activity-info">
              {inEditingMode ? (
                <div className="input-group" style={{ marginBottom: '80px' }}>
                  {props.data.category == 2 ? (
                    <>
                      <div
                        className="strenght-exercises-container"
                        style={{ height: '250px' }}
                      >
                        {dataCopy.exerciseList.map((obj, index) => (
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
                          onClick={() => handleAddButtonClick()}
                        >
                          <FontAwesomeIcon
                            icon={faPlus}
                            color="white"
                            size="1x"
                          />
                        </button>
                      </div>
                    </>
                  ) : (
                    <>
                      <div className="duration-picker-container">
                        {/* <p>Duration:</p> */}
                        <div className="duration-picker">
                          <div className="hours">
                            <p>Hours</p>
                            <input
                              value={dataCopy.durationHours}
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
                              value={dataCopy.durationMinutes}
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
                          value={dataCopy.description}
                          onChange={(e) =>
                            setDataCopy({
                              ...dataCopy,
                              description: e.target.value,
                            })
                          }
                        ></textarea>
                      </div>
                    </>
                  )}
                </div>
              ) : (
                <>
                  {props.data.category == 2 ? (
                    <>
                      <div className="exercises-table-container">
                        <table className="table table-hover table-sm">
                          <thead className="thead-dark">
                            <tr>
                              <th colSpan="3" scope="col">
                                EXERCISE
                              </th>
                              <th
                                colSpan="1"
                                scope="col"
                                style={{ textAlign: 'center', width: '15%' }}
                              >
                                SETS
                              </th>
                              <th
                                colSpan="1"
                                scope="col"
                                style={{ textAlign: 'center', width: '15%' }}
                              >
                                REPS
                              </th>
                            </tr>
                          </thead>
                          <tbody>
                            {dataCopy.exerciseList.map((obj, index) => (
                              <tr>
                                <td colSpan="3">{obj.exerciseName}</td>
                                <td
                                  colSpan="1"
                                  style={{ textAlign: 'center', width: '15%' }}
                                >
                                  {obj.sets}
                                </td>

                                <td
                                  colSpan="1"
                                  style={{ textAlign: 'center', width: '15%' }}
                                >
                                  {obj.reps}
                                </td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    </>
                  ) : (
                    <>
                      {props.data.durationHours == null &&
                      props.data.durationMinutes == null ? null : (
                        <div className="duration-container">
                          <h1>Duration: </h1>
                          <div>
                            {props.data.durationHours ? (
                              <>
                                <p>0{props.data.durationHours}:</p>
                                {props.data.durationMinutes == null ? (
                                  <p>00</p>
                                ) : (
                                  <>
                                    {props.data.durationMinutes < 10 ? (
                                      <p>0{props.data.durationMinutes}</p>
                                    ) : (
                                      <p>{props.data.durationMinutes}</p>
                                    )}
                                  </>
                                )}
                              </>
                            ) : (
                              <p>{props.data.durationMinutes} Minutes</p>
                            )}
                          </div>
                        </div>
                      )}
                      <div className="description-container">
                        {/* <h1>Description :</h1> */}
                        <p>{props.data.description}</p>
                        <div></div>
                      </div>
                    </>
                  )}
                </>
              )}
              {inEditingMode ? (
                <div className="edit-delete-buttons">
                  <button
                    onClick={() => handleCancelClick()}
                    type="button"
                    className="btn btn-outline-secondary"
                  >
                    Cancel
                  </button>
                  <button
                    onClick={() => updateActivity()}
                    type="button"
                    className="btn btn-dark"
                  >
                    Save
                  </button>
                </div>
              ) : (
                <div className="edit-delete-buttons">
                  <button
                    className="rawbutton"
                    onClick={(e) => handleEditButtonClick()}
                  >
                    <FontAwesomeIcon
                      icon={faPencil}
                      color={activityColor[props.data.category]}
                      size="lg"
                    />
                  </button>

                  <button
                    className="rawbutton"
                    onClick={(e) => deleteActivity(props.data.id)}
                  >
                    <FontAwesomeIcon icon={faTrashCan} color="red" size="lg" />
                  </button>
                </div>
              )}
            </div>
          </div>
          <></>
        </div>
      </div>
    </>
  );
}

export default ViewActivityContainer;
