import { ToastModule } from 'primeng/toast';
import { MultiSelectModule } from 'primeng/multiselect';
import { AdminRoutingModule } from './admin-routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule as formModule, ReactiveFormsModule } from '@angular/forms';
import { NgxMaskModule } from 'ngx-mask';
import { NgSelectModule } from '@ng-select/ng-select';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { ArchwizardModule } from 'angular-archwizard';
import { CustomFormsModule } from 'ngx-custom-validators';
import { ExcelUploadComponent } from './excel-upload/excel-upload.component';
import { RippleModule } from 'primeng/ripple';
import { ReportGenarateComponent } from './report-genarate/report-genarate.component';



@NgModule({
  declarations: [
    ExcelUploadComponent,
    ReportGenarateComponent,
   
    
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    NgxMaskModule.forRoot(),
    NgSelectModule,
    CKEditorModule,
    ArchwizardModule,
    CustomFormsModule,
    MultiSelectModule,
    formModule,
    ReactiveFormsModule,
    RippleModule,
    ToastModule
    
  ]
})
export class AdminModule { }
