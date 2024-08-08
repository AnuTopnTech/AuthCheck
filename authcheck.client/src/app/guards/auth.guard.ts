import { state } from "@angular/animations";
import { inject } from "@angular/core";
import {  CanActivateFn } from "@angular/router";
import { AuthService } from "../services/auth.service";
import { MatSnackBar } from "@angular/material/snack-bar";

export const authGuard: CanActivateFn = (route, state) => {
  const matSnackbar = inject(MatSnackBar);

  if (inject(AuthService).isLoggedIn()) {
    return true;
  }
  matSnackbar.open('You must be logged in to view this page', 'Ok', {
    duration: 3000,
  });
  return false;
};
