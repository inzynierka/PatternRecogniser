import { useState, useEffect } from 'react';

function getWindowDimensions() {
  const { innerWidth: width, innerHeight: height } = window;
  return {
    width,
    height
  };
}

function checkOrientationIfVertical() {
  const { innerWidth: width, innerHeight: height } = window;
  return !(height < 768 || width < height);
}

export default function useWindowDimensions() {
  const [ , setWindowDimensions] = useState(getWindowDimensions());
  const [isOrientationVertical, setIsOrientationVertical] = useState(checkOrientationIfVertical());

  useEffect(() => {
    function handleResize() {
      setWindowDimensions(getWindowDimensions());
      setIsOrientationVertical(checkOrientationIfVertical())
    }

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  return isOrientationVertical;
}