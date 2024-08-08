import { Component , inject} from '@angular/core';

import { AuthService } from '../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrl: './forget-password.component.scss'
})
export class ForgetPasswordComponent {
  email!: string;
  authService = inject(AuthService);
  matsnackbar = inject(MatSnackBar);
  showEmailSent = false;
  isSubmitting = false;

  forgetPassword() {
    this.isSubmitting = true;
    this.authService.forgetPassword(this.email).subscribe({
      next: (response) => {
        if (response.isSuccess) {
          this.matsnackbar.open(response.message, 'Close', {
            duration: 5000
          })
          this.showEmailSent = true;
        } else {
          this.matsnackbar.open(response.message, 'Close', { duration: 5000, });
        }
      },
      error: (error: HttpErrorResponse) => {
        this.matsnackbar.open(error.message, 'Close', {
          duration: 5000,
        });
      },
      complete: () => {
        this.isSubmitting = false;
      }
    })
  }
}
