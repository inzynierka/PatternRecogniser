import { message } from 'antd';
import { useState } from 'react';

export enum LogOutReason {
    userLoggedOut = "userLoggedOut",
    tokenExpired = "tokenExpired",
    error = "error"
  }  


export const LogOut = async (reason : string) => {
    localStorage.clear();
    let displayMessage = reason === 'tokenExpired' ? 'Sesja wygasła. Zaloguj się ponownie.'
        : reason === 'userLoggedOut' ? 'Wylogowano pomyślnie.'
        : 'Wystąpił błąd. Spróbuj ponownie później';

    message.success(displayMessage);
}