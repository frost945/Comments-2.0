import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class FileDownloadService {
  
  private txtFileUrl = environment.fileDownloadUrl+"/text/";

  constructor(private http: HttpClient) {}

  downloadFile(id: string, fileName: string) {
  this.http.get(`${this.txtFileUrl}${id}`, { responseType: 'blob' })
    .subscribe(blob => {
      const url = window.URL.createObjectURL(blob);

      const a = document.createElement('a');
      a.href = url;
      a.download = fileName || 'file.txt';
      a.click();

      window.URL.revokeObjectURL(url);  
    });
    }
}