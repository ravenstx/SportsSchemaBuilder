import React, { useState, useEffect } from 'react';
import { API_URL } from '../util/Urls';
import '../css/Analysis.css';
import FileUploader from './FileUploader';
import ActivitiesList from './ActivitiesList';
import AnalysisDataBoard from './AnalysisDataBoard';

function Analysis() {
  const [fileNamesArray, setFileNamesArray] = useState([]);
  const [selectedFileIndex, setSelectedFileIndex] = useState();
  const [fitFileData, setFitFileData] = useState();
  const [fileUploadChange, setFileUploadChange] = useState();

  useEffect(() => {
    if (selectedFileIndex != null) {
      const id = fileNamesArray[selectedFileIndex].id;
      download(id);
    }
  }, [selectedFileIndex]);
  const download = async (id) => {
    try {
      const res = await fetch(API_URL + `/api/FitFiles/Download/${id}`, {
        credentials: 'include',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },
      });
      if (res.ok) {
        let reader = new FileReader(); // no arguments
        const blob = await res.blob();
        const arr = await blob.arrayBuffer();
        parseFitFile(arr);
      }
    } catch (error) {
      console.log(error);
    }
  };

  const parseFitFile = (arrayBuffer) => {
    var FitParser = require('fit-file-parser').default;
    // Create a FitParser instance (options argument is optional)
    var fitParser = new FitParser({
      force: true,
      speedUnit: 'km/h',
      lengthUnit: 'km',
      temperatureUnit: 'celcius',
      elapsedRecordField: true,
      mode: 'cascade',
    });

    // Parse your file
    fitParser.parse(arrayBuffer, function (error, data) {
      // Handle result of parse method
      if (error) {
        console.log(error);
      } else {
        //console.log(JSON.stringify(data));
        setFitFileData(data);
      }
    });
  };
  return (
    <div className="analysis">
      <div className="activities-and-upload">
        <ActivitiesList
          fileNamesArray={fileNamesArray}
          setFileNamesArray={setFileNamesArray}
          selectedFileIndex={selectedFileIndex}
          setSelectedFileIndex={setSelectedFileIndex}
          fileUploadChange={fileUploadChange}
          setFileUploadChange={setFileUploadChange}
        />

        <div className="upload">
          <FileUploader setFileUploadChange={setFileUploadChange} />
        </div>
      </div>
      {fitFileData != null ? (
        <AnalysisDataBoard
          fitFileData={fitFileData}
          selectedFileIndex={selectedFileIndex}
        />
      ) : null}
    </div>
  );
}

export default Analysis;
