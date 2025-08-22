import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {DeviceActivity} from 'src/app/models/device-activity';
import {DeviceActivityService} from 'src/app/services/device-activity.service';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.less']
})
export class DeviceDetailComponent implements OnInit {
  device: DeviceActivity|null = null;

  constructor(
      private route: ActivatedRoute,
      private deviceActivityService: DeviceActivityService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    // null check для ID, чтобы избежать ошибок, если ID не найдено в URL
    if (id) {
      this.deviceActivityService.getDeviceById(id).subscribe(
          (device: DeviceActivity) => {
            this.device = device;
          })
    }
  }
}
