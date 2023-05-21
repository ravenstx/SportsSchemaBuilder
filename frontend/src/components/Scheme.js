import React, { useState, useEffect } from 'react';
import { API_URL } from '../util/Urls';
import '../css/Scheme.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { faChevronRight } from '@fortawesome/free-solid-svg-icons';
import { faDumbbell } from '@fortawesome/free-solid-svg-icons';
import { faPersonBiking } from '@fortawesome/free-solid-svg-icons';
import { faPersonRunning } from '@fortawesome/free-solid-svg-icons';
import { faPersonSwimming } from '@fortawesome/free-solid-svg-icons';

import CalendarEditContainer from './CalendarEditContainer';
import ViewActivityContainer from './ViewActivityContainer';

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

const activityClassNames = [
  'cyclingActivity',
  'runningActivity',
  'strengthActivity',
  'swimmingActivity',
];
const activityNames = ['Cycling', 'Running', 'Strength', 'Swimming'];

const activityIcons = [
  faPersonBiking,
  faPersonRunning,
  faDumbbell,
  faPersonSwimming,
];

const daysInMonth = (year, month) => new Date(year, month, 0).getDate();

function Scheme() {
  const [currentDateForCalendar, setcurrentDateForCalendar] = useState(
    new Date()
  );
  const [editingDate, setEditingDate] = useState();
  const [activityData, setActivityData] = useState();
  const [calendarDaysArray, setCalendarDaysArray] = useState([]);
  const [monthName, setMonthName] = useState();
  const [currentDay, setCurrentDay] = useState();
  const [inEditingMode, setInEditingMode] = useState(false);
  const [reloadUserData, setReloadUserData] = useState(false);
  const [inViewActivityMode, setInViewActivityMode] = useState(false);
  const [calendarUserData, setCalendarUserData] = useState([]);

  useEffect(() => {
    const daysArr = loadCalendarArray(
      new Date(
        currentDateForCalendar.getFullYear(),
        currentDateForCalendar.getMonth(),
        1
      ).getDay(),
      currentDateForCalendar
    );
    setMonthName(
      `${
        months[currentDateForCalendar.getMonth()]
      } ${currentDateForCalendar.getFullYear()}`
    );

    getCalendarUserData(daysArr);
  }, [currentDateForCalendar]);

  useEffect(() => {
    if (!reloadUserData) {
      return;
    }
    const daysArr = loadCalendarArray(
      new Date(
        currentDateForCalendar.getFullYear(),
        currentDateForCalendar.getMonth(),
        1
      ).getDay(),
      currentDateForCalendar
    );
    setMonthName(
      `${
        months[currentDateForCalendar.getMonth()]
      } ${currentDateForCalendar.getFullYear()}`
    );

    getCalendarUserData(daysArr);

    setReloadUserData(false);
  }, [reloadUserData]);

  const loadCalendarArray = (firstMonthDay, date) => {
    if (firstMonthDay == 0) {
      firstMonthDay = 7;
    }
    let lastMonthDay;
    let res = [];
    const totalDays = daysInMonth(date.getFullYear(), date.getMonth() + 1);
    if (firstMonthDay > 1) {
      let i = 1;
      while (i < firstMonthDay) {
        const obj = {
          date: new Date(
            date.getFullYear(),
            date.getMonth(),
            i - (firstMonthDay - 1)
          ),
          isCurrentMonth: false,
        };
        res.push(obj);
        i++;
      }
    }

    for (let i = 1; i < totalDays + 1; i++) {
      const obj = {
        date: new Date(date.getFullYear(), date.getMonth(), i),
        isCurrentMonth: true,
      };
      res.push(obj);

      if (i == totalDays) {
        lastMonthDay = new Date(
          date.getFullYear(),
          date.getMonth(),
          i
        ).getDay();
      }
    }

    if (lastMonthDay > 0) {
      for (let i = 1; i < 8 - lastMonthDay; i++) {
        const obj = {
          date: new Date(date.getFullYear(), date.getMonth(), i + totalDays),
          isCurrentMonth: false,
        };
        res.push(obj);
      }
    }

    const arr = chunk(res);

    setCalendarDaysArray(arr);
    return arr;
  };

  function chunk(arr) {
    let chunks = [];
    const len = arr.length;
    for (let i = 0; i < len; i += 7) {
      chunks.push(arr.slice(i, i + 7));
    }

    return chunks;
  }

  const getCalendarUserData = async (daysArr) => {
    const firstDay = daysArr[0][0].date;
    const lastDay =
      daysArr[daysArr.length - 1][daysArr[daysArr.length - 1].length - 1].date;

    const firstDate = new Date(
      firstDay.getTime() - firstDay.getTimezoneOffset() * 60000
    )
      .toISOString()
      .split('T')[0];

    const lastDate = new Date(
      lastDay.getTime() - lastDay.getTimezoneOffset() * 60000
    )
      .toISOString()
      .split('T')[0];

    try {
      const res = await fetch(
        API_URL + `/api/Calendar/CalendarData/${firstDate}/${lastDate}`,
        {
          credentials: 'include',
          headers: {
            Authorization: `Bearer ${localStorage['user-token']}`,
            accept: '*/*',
          },
        }
      );
      if (res.ok) {
        const data = await res.json();
        if (data.activities.length == 0) {
          setCalendarUserData(data.activities);
        } else {
          setCalendarUserData(data.activities);
        }
      }
    } catch (error) {
      console.log(error);
    }
  };

  const addActivity = (obj) => {
    setEditingDate(obj.date);
    setInEditingMode(true);
  };

  const handleActivityClick = (e, index, obj, data) => {
    e.stopPropagation();
    setActivityData(data);
    setInViewActivityMode(true);
  };

  const handleBackButton = () => {
    if (inEditingMode) {
      return;
    }
    if (currentDateForCalendar.getMonth() == 0) {
      setMonthName(`${months[11]} ${currentDateForCalendar.getFullYear() - 1}`);
    } else {
      setMonthName(
        `${
          months[currentDateForCalendar.getMonth() - 1]
        } ${currentDateForCalendar.getFullYear()}`
      );
    }
    setcurrentDateForCalendar(
      new Date(
        currentDateForCalendar.getFullYear(),
        currentDateForCalendar.getMonth() - 1,
        1
      )
    );
  };

  const handleForwardButton = () => {
    if (inEditingMode) {
      return;
    }
    if (currentDateForCalendar.getMonth() == 11) {
      setMonthName(`${months[0]} ${currentDateForCalendar.getFullYear() + 1}`);
    } else {
      setMonthName(
        `${
          months[currentDateForCalendar.getMonth() + 1]
        } ${currentDateForCalendar.getFullYear()}`
      );
    }
    setcurrentDateForCalendar(
      new Date(
        currentDateForCalendar.getFullYear(),
        currentDateForCalendar.getMonth() + 1,
        1
      )
    );
  };

  return (
    <>
      <div className="scheme">
        <div className="calendar">
          {inEditingMode ? (
            <CalendarEditContainer
              date={editingDate}
              setInEditingMode={setInEditingMode}
              setReloadUserData={setReloadUserData}
            />
          ) : null}
          {inViewActivityMode ? (
            <ViewActivityContainer
              data={activityData}
              setInViewActivityMode={setInViewActivityMode}
              setReloadUserData={setReloadUserData}
            />
          ) : null}
          <div className="calendar-info">
            <div className="month-name">{monthName}</div>
            <div className="arrows">
              <button
                className="back-button"
                onClick={() => handleBackButton()}
              >
                <FontAwesomeIcon icon={faChevronLeft} color="black" size="1x" />
              </button>
              <button
                className="forward-button"
                onClick={() => handleForwardButton()}
              >
                <FontAwesomeIcon
                  icon={faChevronRight}
                  color="black"
                  size="1x"
                />
              </button>
            </div>
          </div>
          <table className="calendar-table">
            <thead className="calendar-head">
              <tr>
                <th>Mon</th>
                <th>Tue</th>
                <th>Wed</th>
                <th>Thu</th>
                <th>Fri</th>
                <th>Sat</th>
                <th>Sun</th>
              </tr>
            </thead>
            <tbody>
              {calendarDaysArray.map((arr, index) => {
                return (
                  <tr key={index}>
                    {arr.map((obj, i) => {
                      return (
                        <td
                          key={i}
                          className={
                            obj.isCurrentMonth == false
                              ? 'not-current-month'
                              : ''
                          }
                        >
                          <div className="day" onClick={() => addActivity(obj)}>
                            <p className="date-number">{obj.date.getDate()}</p>

                            <div className="activities-container">
                              {calendarUserData.map((data, index) => {
                                if (
                                  obj.date.getTime() ===
                                  new Date(data.date).getTime()
                                ) {
                                  return (
                                    <div
                                      key={data.id}
                                      onClick={(e) =>
                                        handleActivityClick(e, index, obj, data)
                                      }
                                      className={`activity ${
                                        activityClassNames[data.category]
                                      }`}
                                    >
                                      <FontAwesomeIcon
                                        icon={activityIcons[data.category]}
                                        color="white"
                                        size="xs"
                                      />
                                      <p>{activityNames[data.category]}</p>
                                    </div>
                                  );
                                } else {
                                  return null;
                                }
                              })}
                            </div>
                          </div>
                        </td>
                      );
                    })}
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}

export default Scheme;
