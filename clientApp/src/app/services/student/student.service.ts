import { HttpClient } from '@angular/common/http';
import { StudentModel } from './../../models/student/student.model'
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseModel } from 'src/app/models/common/response.model';

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  constructor(private httpClient: HttpClient) { }

  getAll(): Observable<StudentModel[]> {
    return this.httpClient.
      get<StudentModel[]>(environment.apiUrl + 'Student');
  }

  save(vm: StudentModel): Observable<ResponseModel> {
    return this.httpClient.
      post<ResponseModel>(environment.apiUrl + 'Student', vm);
  }

  delete(id: number): Observable<ResponseModel> {
    return this.httpClient.
      delete<ResponseModel>(environment.apiUrl + 'Student/' + id);
  }
}