import { makeAutoObservable, runInAction } from 'mobx';
import agent from '../api/agent';
import { Activity, ActivityFormValues } from '../models/activity';

import { format } from 'date-fns';
import { store } from './store';
import { Profile } from '../models/profile';

export default class ActivityStore {
  // activities: Activity[] = [];

  activityRegistry = new Map<string, Activity>();
  selectedActivity: Activity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this);
  }

  get activitiesByDate() {
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => a.date!.getTime() - a.date!.getTime()
    );
  }

  get groupedActivities() {
    return Object.entries(
      this.activitiesByDate.reduce((activities, activity) => {
        const date = format(activity.date!, 'dd MMM yyy');
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];
        return activities;
      }, {} as { [key: string]: Activity[] }) //Activity is value.  activity date is key and for each date we are gonna have array of activities inside
    );
  }

  loadActivities = async () => {
    this.setLoadingInitial(true);
    try {
      const activities = await agent.Activities.list(); // get activities from api
      activities.forEach((activity) => {
        // looping over activities
        this.setActivity(activity);
        // this.activities.push(activity); //mutating state  in mobx
      });

      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);
      this.setLoadingInitial(true);
    }
  };

  loadActivity = async (id: string) => {
    let activity = this.getActivity(id);

    if (activity) {
      this.selectedActivity = activity;
      return activity;
    } else {
      this.setLoadingInitial(true);
      try {
        activity = await agent.Activities.details(id);
        this.setActivity(activity);
        runInAction(() => (this.selectedActivity = activity));

        this.setLoadingInitial(false);
        return activity;
      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  };

  private setActivity = (activity: Activity) => {
    const user = store.userStore.user;

    if (user) {
      //some func Determines whether activity.isGoing returns true.
      activity.isGoing = activity.attendees!.some(
        (a) => a.username === user.username // find if this specific user is Going
      );
      activity.isHost = activity.hostUsername === user.username; //determine if is a host

      activity.host = activity.attendees?.find(
        // returns host is a true if attendee username and hostname is the same
        (x) => x.username === activity.hostUsername
      );
    }

    activity.date = new Date(activity.date!); // 0 is first part of split operation
    this.activityRegistry.set(activity.id, activity); // activity.id = key  and activity = value
  };

  private getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };



  createActivity = async (activity: ActivityFormValues) => {

   const user = store.userStore.user;
   const attendee = new Profile(user!);

    try {
      await agent.Activities.create(activity);
      const newActivity = new Activity(activity);
      newActivity.hostUsername = user!.username;
      newActivity.attendees= [attendee];
      this.setActivity(newActivity);
      runInAction(() => {
        //  this.activities.push(activity);
        //this.activityRegistry.set(activity.id, activity); // key = activity.id
        this.selectedActivity = newActivity;
   
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateActivity = async (activity: ActivityFormValues) => {
  
    try {
      await agent.Activities.update(activity);
      runInAction(() => {
        if(activity.id){
          let updatedActivity = {...this.getActivity(activity.id), ...activity}
          this.activityRegistry.set(activity.id, updatedActivity as Activity);
          this.selectedActivity = updatedActivity as Activity;
        }
   
       
       
  
      });
    } catch (error) {
  
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
        this.loading = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  updateAttendance = async () => {
    const user = store.userStore.user; // get user objecz from store
    this.loading = true;

    try {
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        if (this.selectedActivity?.isGoing) {
           this.selectedActivity.attendees = this.selectedActivity.attendees?.filter((a) => a.username !== user?.username);//filter all objects out except with that username 
           this.selectedActivity.isGoing = false; // and finaly set isGoing flag in selectedActivity to false
        }
        else {
            const attendee = new Profile(user!);
            this.selectedActivity?.attendees?.push(attendee); // add new attendee object in selectedActivity attendees array of objects
            this.selectedActivity!.isGoing=true; // finaly set isGoing to true
        }

          this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
      });
    } catch (error) {
      console.log(error);
    } finally {
      // weather we have got  an error or succes we are always finally gonna set loading flag to false
      runInAction(() => (this.loading = false));
    }
  };



  cancelActivityToggle = async () => {
    this.loading = true;
    try {
        await agent.Activities.attend(this.selectedActivity!.id);
        runInAction(() => {
            this.selectedActivity!.isCancelled = !this.selectedActivity!.isCancelled; //So we're just using this as to set it to whatever the opposite of what it already is set to
            this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
        })
    } catch (error) {
        console.log(error);
    } finally {
        runInAction(() => this.loading = false);
    }
}


clearSelectedActivity = () => {
  this.selectedActivity = undefined;
}

updateAttendeeFollowing = (username: string) => {
  this.activityRegistry.forEach(activity => {
      activity.attendees.forEach((attendee: Profile) => {
          if (attendee.username === username) {
              attendee.following ? attendee.followersCount-- : attendee.followersCount++;
              attendee.following = !attendee.following;
          }
      })
  })
}




}
