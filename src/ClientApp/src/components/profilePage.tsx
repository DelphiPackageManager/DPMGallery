import React from 'react';
import {useParams } from 'react-router-dom'


const ProfilePage = () => {
    let { userName } = useParams();
    //todo : use same code from packages page.
    return (
        <>
            <h1>{userName}</h1>
        </>
    );
};

export default ProfilePage;


