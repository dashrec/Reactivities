import React, { useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Header, Icon } from 'semantic-ui-react';

interface Props {
  setFiles: (files: any) => void;
}
// step one drop area component
export default function PhotoUploadWidgetDropzone({ setFiles }: Props) { //get setFiles func from PhotoUploadWidget
  const dzStyles = {
    border: 'dashed 3px #eee',
    borderColor: '#eee',
    borderRadius: '5px',
    paddingTop: '30px',
    textAlign: 'center' as 'center',
    height: '200px',
  };

  const dzActive = {
    borderColor: 'green',
  };
//  URL.createObjectURL(file) <- this is gonna hang around in the clients memory unless we dispose it once we are done with it
  const onDrop = useCallback( // 
    (acceptedFiles: any) => {
      setFiles(acceptedFiles.map((file: any) =>
          Object.assign(file, { // add additional preview  property to file
            preview: URL.createObjectURL(file), //  this is going to give us a preview of the image inside file array that we drop in to our drop zone.
          }) 
        )
      );
    },
    [setFiles]
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <div {...getRootProps()} style={isDragActive ? { ...dzStyles, ...dzActive } : dzStyles}>
      <input {...getInputProps()} />
      <Icon name="upload" size="big" />
      <Header content="Drop image here" />
    </div>
  );
}
