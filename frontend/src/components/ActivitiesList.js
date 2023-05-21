import React, { useState, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashCan } from '@fortawesome/free-solid-svg-icons';
import { API_URL } from '../util/Urls';
import '../css/ActivitiesList.css';

function ActivitiesList(props) {
  const [emptyFitFilesResponse, setEmptyFitFilesResponse] = useState();
  useEffect(() => {
    if (props.fileNamesArray.length == 0) {
      GetFileNames();
    }
    if (props.fileUploadChange == true) {
      GetFileNames();
      props.setFileUploadChange(false);
    }
  }, [props.fileUploadChange]);

  const GetFileNames = async () => {
    try {
      const res = await fetch(API_URL + '/api/FitFiles/Fitfiles', {
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      const data = await res.json();
      if (data.length == 0) {
        setEmptyFitFilesResponse(true);
      } else {
        setEmptyFitFilesResponse(false);
      }
      props.setFileNamesArray(data);
    } catch (error) {
      console.log(error);
      // refreshjwt();
    }
  };
  const handleActivityClick = (id, index) => {
    props.setSelectedFileIndex(index);
  };

  async function deleteFile(id) {
    try {
      const res = await fetch(API_URL + `/api/FitFiles/Delete/${id}`, {
        method: 'DELETE',
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      if (res.ok) {
        const data = await res.json();
        props.setFileUploadChange(true);
        props.setSelectedFileIndex(null);
      }
    } catch (error) {
      console.log(error);
    }
  }
  return (
    <div className="activities">
      {emptyFitFilesResponse ? (
        <div className="alert-container">
          <div className="alert alert-dark" role="alert">
            You have no fit files uploaded yet!
          </div>
        </div>
      ) : (
        <table className="table table-hover">
          <tbody>
            {props.fileNamesArray.map((file, index) => (
              <tr
                className={
                  props.selectedFileIndex == index ? 'table-primary' : ''
                }
                key={index}
                onClick={(e) => handleActivityClick(file.id, index)}
              >
                <th scope="row">{index + 1}</th>
                <td>{file.title}</td>
                <td>{file.filename}</td>

                {props.selectedFileIndex == index ? (
                  <td
                    className="delete"
                    style={{
                      color: '#5F5F5F',
                      textAlign: 'center',
                    }}
                  >
                    <FontAwesomeIcon
                      icon={faTrashCan}
                      color="#414245"
                      size="1x"
                      onClick={(e) => deleteFile(file.id)}
                    />
                  </td>
                ) : (
                  <td style={{ color: 'transparent' }}>
                    <FontAwesomeIcon
                      icon={faTrashCan}
                      color="transparent"
                      size="1x"
                    />
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default ActivitiesList;
