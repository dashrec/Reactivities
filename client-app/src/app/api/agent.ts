import axios, { AxiosError, AxiosResponse } from 'axios';
import { toast } from 'react-toastify';
import { Activity, ActivityFormValues } from '../models/activity';
import { PaginatedResult } from '../models/pagination';
import { Photo, Profile, UserActivity } from '../models/profile';
import { User, UserFormValues } from '../models/user';
import { router } from '../router/Routes';
import { store } from '../stores/store';

const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

// So for every request now  we  have a token, we are going to add this token to our headers as an authorization header.
axios.interceptors.request.use((config) => {
  const token = store.commonStore.token;
  if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axios.interceptors.response.use(
  async (response) => {
    if (process.env.NODE_ENV === 'development') await sleep(1000);

    const pagination = response.headers['pagination'];
    if (pagination) {
      response.data = new PaginatedResult(
        response.data,
        JSON.parse(pagination)
      );
      return response as AxiosResponse<PaginatedResult<any>>;
    }

    return response;
  },
  (error: AxiosError) => {
    const { data, status, config } = error.response as AxiosResponse;

    switch (status) {
      case 400:
        if (config.method === 'get') {
          toast.error(data);
        }

        if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
          router.navigate('/not-found'); // we simply don't have an activity that matches something that is not a valid guid! so send them to not findPage rather than telling that what they used wasn't a valid guid.
        }

        if (data.errors) {
          const modalStateErrors = [];
          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key]);
            }
          }
          throw modalStateErrors.flat();
        } else {
          toast.error(data);
        }

        break;
      case 401:
        toast.error('unauthorised');
        break;
      case 403:
        toast.error('forbidden');
        break;

      case 404:
        router.navigate('/not-found');
        break;

      case 500:
        store.commonStore.setServerError(data);
        router.navigate('/server-error');
        break;
    }
    return Promise.reject(error);
  }
);

//  we're not just going to be getting back activities. So what we need to do is think about making this a generic type of response. ref to T

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) =>
    axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Activities = {
  list: (params: URLSearchParams) =>
    axios
      .get<PaginatedResult<Activity[]>>('/activities', { params })
      .then(responseBody),
  details: (id: string) => requests.get<Activity>(`/activities/${id}`),
  create: (activity: ActivityFormValues) =>
    axios.post<void>('/activities', activity),
  update: (activity: ActivityFormValues) =>
    axios.put<void>(`/activities/${activity.id}`, activity),
  delete: (id: string) => axios.delete<void>(`/activities/${id}`),
  attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {}), // id of the activity. go to activityStore to make an use of it
};

const Account = {
  current: () => requests.get<User>('account'), // get <User> object from this particular request from the url account
  login: (user: UserFormValues) => requests.post<User>('/account/login', user), // this is gonna take value we re getting from our form and pass that user
  register: (user: UserFormValues) =>
    requests.post<User>('/account/register', user),
};

const Profiles = {
  get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
  uploadPhoto: (file: Blob) => {
    let formData = new FormData();
    formData.append('File', file); // File should match the property in our api

    return axios.post<Photo>('photos', formData, {
      // this is gonna return type of Photo
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  setMainPhoto: (id: string) => axios.post(`/photos/${id}/setMain`, {}), //setMain must match the endpoint ouf our api
  deletePhoto: (id: string) => axios.delete(`/photos/${id}`),
  updateProfile: (profile: Partial<Profile>) =>
    requests.put(`/profiles`, profile),
  updateFollowing: (username: string) =>
    requests.post(`/follow/${username}`, {}),
  listFollowings: (username: string, predicate: string) =>
    requests.get<Profile[]>(`/follow/${username}?predicate=${predicate}`), // we are gonna be returning list of Profile

  listActivities: (username: string, predicate: string) =>
    requests.get<UserActivity[]>(
      `/profiles/${username}/activities?predicate=${predicate}`
    ), // send predicate as add on query string
};

const agent = {
  Activities,
  Account,
  Profiles,
};

export default agent;
