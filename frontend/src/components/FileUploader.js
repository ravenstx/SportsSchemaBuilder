import React, { useRef, useState, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowUpFromBracket } from '@fortawesome/free-solid-svg-icons';
import { API_URL } from '../util/Urls';
import '../css/FileUploader.css';

const FileUploader = (props) => {
  const [selectedFile, setSelectedFile] = useState(null);
  const [imageUrl, setImageUrl] = useState(null);
  const [isUploading, setIsUploading] = useState();

  useEffect(() => {
    if (selectedFile && imageUrl == null) {
      upload(selectedFile);
      setSelectedFile(null);
    }
  }, [selectedFile]); // mounts for the first time

  async function upload(file) {
    let formData = new FormData();

    formData.append('file', file);
    setIsUploading(true);

    try {
      const res = await fetch(API_URL + '/api/FitFiles/Upload', {
        method: 'POST',
        credentials: 'include',
        mode: 'cors',
        headers: {
          Authorization: `Bearer ${localStorage['user-token']}`,
          accept: '*/*',
        },

        body: formData,
      });
      if (res.ok) {
        const data = await res.json();
        console.log(data);
        props.setFileUploadChange(true);
      }
    } catch (error) {
      console.log(error);
    }
    setIsUploading(false);
  }

  return (
    <div className="file-container">
      <label className="custom-file-upload" htmlFor="file-upload">
        {isUploading ? (
          <div id="wrapper">
            <p>Uploading</p>
          </div>
        ) : (
          <>
            <p>Upload .fit files</p>
            <FontAwesomeIcon
              icon={faArrowUpFromBracket}
              color="#396ab8"
              size="4x"
            />
            <input
              id="file-upload"
              type="file"
              accept="*"
              onChange={(e) => setSelectedFile(e.target.files[0])}
            />
          </>
        )}
      </label>
    </div>
  );
};

export default FileUploader;
