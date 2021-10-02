import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
// import { Board } from 'src/app/models/board.model';
import { Column } from 'src/app/models/column.model';
import { HttpClient } from '@angular/common/http';
import Swal from 'sweetalert2'
import { timer, Subject } from 'rxjs';
import { Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ChangeDateFormat } from "../../utils/time";
import { AllocatorService } from 'src/app/services/allocator/allocator.service';
@Component({
  selector: 'app-drag',
  templateUrl: './drag.component.html',
  styleUrls: ['./drag.component.scss']
})
export class DragComponent implements OnInit {
  @ViewChild("toggleElement") ref: ElementRef;
  drag = true;
  all = [];
  connect = []

  timeLeft: number = 100000;
  interval;
  subscribeTimer: any;

  // current_datetime = new Date().toSt;



  drop(event: CdkDragDrop<string[]>, name: string, num: number) {



    if (event.previousContainer === event.container) {

      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      if (event.container.data.length < 14) {
        transferArrayItem(event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex);

        if (event.container.id != event.container.data[event.currentIndex]["Subregion"]) {
          event.container.data[event.currentIndex]["color"] = 'moved'
        } else {
          event.container.data[event.currentIndex]["color"] = null
        }

      } else {
        Swal.fire({
          title: name + " Van " + num + " is full",
          icon: "warning",
          html: 'Please choose another van',

        })
      }
    }
  }


  constructor(private http: HttpClient,private allocatorService:AllocatorService) { }
  private sub: Subscription;
  ngOnInit() {
    // alert(this.current_datetime);
    const d  = new Date()
    // console.log(d.format("dd/MM/yyyy hh:mm TT"));
    // this.http.get('http://worldtimeapi.org/api/timezone/Indian/Mauritius').subscribe(d => {
    // this.current_datetime = d["datetime"].slice(11,15);
    // console.log(this.current_datetime)

    // },err => {
    //   // this.current_datetime = new Date("October 13, 2014 17:00:00");
    // })


    this.interval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.timeLeft = 0;

      }
    }, 1000)

    setTimeout(() => {
      this.ableDrag();
      Swal.fire({
        title: "Time limit reached",
        icon: "error",
        html: 'Transport Allocation list have been send to the supplier',

      })
    }, this.timeLeft * 1000);

    this.allocatorService.getAllocations().subscribe((data: any) => {
      // console.log(data);
      var data2 = data.sort((x, y) => x.region < y.region ? -1 : x > y ? 1 : 0);
      var lastRegion;
      var n = 0;
      // this.connect = data.map(a => a.region)
      (data2).forEach(region => {
        if (lastRegion == null)
          lastRegion = region.region
        n = (region.region != lastRegion) ? 1 : n + 1;
        lastRegion = region.region
        const col = new Column(n, (region.timestamp),region.region, region.subregions[0], region.passengers);

        this.all.push(col);
        this.connect.push(region.subregions[0]);
        // console.log(col);
      });
      // console.log(this.connect);
    })
  }

  save() {

    let response = this.all.map(a => {
      return {
        passengers: a.tasks,
        passengers_count: a.tasks.length,
        region: a.name,
        timestamp: a.timestamp,
        subregions: [a.sub]
      }
    })

    this.allocatorService.postAllocations(response).subscribe(data => {
      console.log(data)
    },err => {
      console.log(err.message)
    })


  }
  reload() {
    window.location.reload();
  }
  ableDrag() {
    if (this.drag==true) {
      this.drag = false
    }
    else { this.drag = true }
  }
}



