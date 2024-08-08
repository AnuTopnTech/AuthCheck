import { HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { LoginComponent } from './pages/login.component';
import { AppMaterialModule } from './app-material-module';
import { NavComponent } from './navbar/nav.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { AccountComponent } from './pages/account/account.component';
import { tokenInterceptor } from './interceptor/token.interceptor';
import { UsersComponent } from './pages/users/users.component';
import { ForgetPasswordComponent } from './pages/forget-password/forget-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { CityComponent } from './pages/city/city.component';
import { AllLocationsComponent } from './all-locations/all-locations.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    AccountComponent,
    UsersComponent,
    ForgetPasswordComponent,
    ResetPasswordComponent,
    ChangePasswordComponent,
    CityComponent,
    AllLocationsComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule, AppMaterialModule
  ],
  providers: [
    provideHttpClient(withInterceptors([tokenInterceptor])),
    provideAnimationsAsync(),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
