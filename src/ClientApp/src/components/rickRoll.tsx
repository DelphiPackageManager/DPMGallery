import React, { useEffect } from "react";

const RickRoll = () => {
  useEffect(() => {
    window.location.href = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
  }, []);

  return <h5>Nice try...</h5>;
};

export default RickRoll;
