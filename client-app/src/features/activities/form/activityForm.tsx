import React, { useState, ChangeEvent } from 'react';
import { Button, Form, Segment } from 'semantic-ui-react';
import { Activity } from '../../../app/models/activity';

interface Props {
  activity: Activity | undefined;
  closeForm: () => void;
  createOrEdit: (activity: Activity) => void;
  submitting: boolean;
}

export default function ActivityForm({
  activity: selectedActivity,
  closeForm,
  createOrEdit,
  submitting
}: Props) {
  const initialState = selectedActivity ?? {
    id: '',
    title: '',
    category: '',
    description: '',
    date: '',
    city: '',
    venue: '',
  };


  const [activity, setActivity] = useState(initialState);

  function handleSubmit() {
  createOrEdit(activity);
  }

  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement >) {
    const { name, value } = event.target; // destructure name and value from event
    setActivity({ ...activity, [name]: value }); //square bracket notation means: property with the key of name should be set to whatever the value is inside the input element
  }

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete="off">
        <Form.Input
          placeholder="Title"
          value={activity.title}
          name="title"
          onChange={handleInputChange}
        />
        <Form.Input
          placeholder="Description"
          value={activity.description}
          name="description"
          onChange={handleInputChange}
        />
        <Form.Input
          placeholder="Category"
          value={activity.category}
          name="category"
          onChange={handleInputChange}
        />
        <Form.Input
          placeholder="Date"
          value={activity.date}
          name="date"
          type='date'
          onChange={handleInputChange}
        />
        <Form.Input
          placeholder="City"
          value={activity.city}
          name="city"
          onChange={handleInputChange}
        />
        <Form.Input
          placeholder="Venue"
          value={activity.venue}
          name="venue"
          onChange={handleInputChange}
        />

        <Button loading={submitting} floated="right" positive type="submit" content="Submit" />
        <Button 
          floated="right"
          positive
          type="button"
          onClick={closeForm}
          content="Cancel"
        />
      </Form>
    </Segment>
  );
}
