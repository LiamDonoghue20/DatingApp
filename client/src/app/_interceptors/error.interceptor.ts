import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error){
          switch (error.status)
          {
            case 400:
            //error = error we get from HttpErrorResponse
            //.error = looking within the body of that HttpErrorResponse
            //.errors = array of errors
              if(error.error.errors){
                const modelStateErrors = [];
                //for each item in the array of errors retrieved from the HttpErrorResponse
                for (const key in error.error.errors){
                  //if the key exists in the errors, push to local variable array of modelStateErrors 
                  if(error.error.errors[key]){
                    modelStateErrors.push(error.error.errors[key])
                  }
                }
                throw modelStateErrors.flat();
              } 
              //if we dont have the error.error.errors object its a normal bad request
              else {
                this.toastr.error(error.error, error.status.toString())
              }
              break;
            case 401:
              this.toastr.error('Unauthorised', error.status.toString())
              break;
            case 404:
              this.router.navigateByUrl('/not-found')
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: { error: error.error}};
              //send the state variables (navigationExtras) along with the URL so the error.error body can be shown on the page
              this.router.navigateByUrl('/server-error', navigationExtras)
              break;
            default:
              this.toastr.error('Something unexpected went wrong')
              console.log(error)
              break;
          }
        }
        throw error;
      })
    )
  }
}
