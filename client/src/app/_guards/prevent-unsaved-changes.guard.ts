import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent) : boolean {
    //if the edit form has been altered then ask the user to confirm before they go to another component
    if (component.editForm.dirty) {
      //this confirm will return either true or false depending on what the user selects
      return confirm('Are you sure you want to continue? Any unsaved changess will be lost')
    }
    //this return is just for the sake of typescript and wont be used, unless confirm breaks somehow
    return true;
  }
  
}
