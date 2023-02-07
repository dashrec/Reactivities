import { makeAutoObservable, runInAction } from 'mobx';
import agent from '../api/agent';
import { Activity } from '../models/activity';
import { v4 as uuid } from 'uuid';

export default class ActivityStore {
  // activities: Activity[] = [];

  activityRegistry = new Map<string, Activity>();

  selectedActivity: Activity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = true;

  constructor() {
    makeAutoObservable(this);
  }
  get activitiesByDate () {
    return Array.from (this.activityRegistry.values()).sort((a, b) => Date.parse(a.date) - Date.parse(b.date));
  }

  loadActivities = async () => {
  
    try {
      const activities = await agent.Activities.list(); // get activities from api
      activities.forEach((activity) => {
        // looping over activities
        activity.date = activity.date.split('T')[0]; // 0 is first part of split operation
        this.activityRegistry.set(activity.id, activity); // activity.id = key  and activity = value
        // this.activities.push(activity); //mutating state  in mobx
      });

      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(true);
    }
  };
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  selectActivity = (id: string) => {
    // this.selectedActivity = this.activities.find((a) => a.id === id);
    this.selectedActivity = this.activityRegistry.get(id);
  };

  cancelSelectedActivity = () => {
    this.selectedActivity = undefined;
  };

  openForm = (id?: string) => {
    id ? this.selectActivity(id) : this.cancelSelectedActivity();
    this.editMode = true;
  };

  closeForm = () => {
    this.editMode = false;
  };

  createActivity = async (activity: Activity) => {
    this.loading = true;
    activity.id = uuid();

    try {
      await agent.Activities.create(activity);
      runInAction(() => {
        //  this.activities.push(activity);
        this.activityRegistry.set(activity.id, activity); // key = activity.id
        this.selectedActivity = activity;
        this.editMode = false;
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateActivity = async (activity: Activity) => {
    this.loading = true;
    try {
      await agent.Activities.update(activity);
      runInAction(() => {
        /*     this.activities = [
          ...this.activities.filter((x) => x.id !== activity.id),
          activity,
        ]; // spread operator creates new array
         */
        this.activityRegistry.set(activity.id, activity);
        this.selectedActivity = activity;
        this.editMode = false;
        this.loading = false;
        // this.activities.push(activity);
      });
    } catch (error) {
      runInAction(() => {
        this.loading = false;
      });
      console.log(error);
    }
  };

  deleteActivity = async (id: string) => {
    this.loading = true;

    try {
      await agent.Activities.delete(id);
      runInAction(() => {
        //  this.activities = [...this.activities.filter((x) => x.id !== id)];
        this.activityRegistry.delete(id);
        if (this.selectedActivity?.id === id) this.cancelSelectedActivity();
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };
}
