import { StudentMCQQuestionAnswerPaginatedItemsViewModel } from './../../models/student-mcq-question-answer/studentmcqquestionanswer.paginated.item';
import { DropDownModel } from './../../models/common/drop-down.model';
import { ResponseModel } from './../../models/common/response.model';
import { environment } from './../../../environments/environment';
import { StudentMcqQuestionAnswerModel } from './../../models/student-mcq-question-answer/student-mcq-question-answer';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StudentMcqQuestionAnswerService {
  constructor( private httpClient: HttpClient ) { }


  getAll(): Observable<StudentMcqQuestionAnswerModel[]>{
    return this.httpClient.
      get<StudentMcqQuestionAnswerModel[]>
      (environment.apiUrl + 'StudentMCQQuestion/getAll');
  }
 
  saveStudentMcqQuestionAnswer(vm: StudentMcqQuestionAnswerModel): Observable<ResponseModel> {
    return this.httpClient.
      post<ResponseModel>
      (environment.apiUrl + 'StudentMCQQuestion', vm);
  }
  
  getAllQuestions():Observable<DropDownModel[]>{
    return this.httpClient.
      get<DropDownModel[]>
      (environment.apiUrl + 'StudentMCQQuestion/getAllQuestions');
  }

  getAllStudentAnswerTexts():Observable<DropDownModel[]>{
    return this.httpClient.
      get<DropDownModel[]>
      (environment.apiUrl + 'StudentMCQQuestion/getAllStudentAnswerTexts');
  }

  getAllStudentNames():Observable<DropDownModel[]>{
    return this.httpClient.
      get<DropDownModel[]>
      (environment.apiUrl + 'StudentMCQQuestion/getAllStudentNames');
  }

  getStudentNameList(searchText: string, currentPage: number, pageSize: number, studentNameId:number,):Observable<StudentMCQQuestionAnswerPaginatedItemsViewModel>{
    return this.httpClient.get<StudentMCQQuestionAnswerPaginatedItemsViewModel>(environment.apiUrl + "StudentMCQQuestion/getStudentNameList",{
      params:new HttpParams()
        .set('searchText',searchText)
        .set('currentPage', currentPage.toString())
        .set('pageSize', pageSize.toString())
        .set('studentNameId', studentNameId.toString())
    });
  }

   /* getStudents(): Observable<StudentMcqQuestionAnswerModel[]>{
    return this.httpClient.
      get<StudentMcqQuestionAnswerModel[]>
      (environment.apiUrl + 'StudentMCQQuestion/getStudents');
    } */ 

    /* report(vm: StudentMcqQuestionAnswerModel): Observable<ResponseModel> {
      return this.httpClient.
        post<ResponseModel>
        (environment.apiUrl + 'StudentMCQQuestion', vm);
    }
 */
  /* report(id: number): Observable <ResponseModel> { 
    return this.httpClient. 
    report<ResponseModel>(environment.apiUrl + 'StudentMCQQuestion/report' + id); 
  } */


/* getStudents(apiUrl) {
  var headers = {};
  headers.Authorization = 'Bearer ' + sessionStorage.tokenKey;
  var deferred = $q.defer();
  $http.get(
      hostApiUrl + apiUrl,
      {
          responseType: 'arraybuffer',
          headers: headers
      })
  .success(function (result, status, headers) {
      deferred.resolve(result);;
  })
   .error(function (data, status) {
       console.log("Request failed with status: " + status);
   });
  return deferred.promise;
} */

}
