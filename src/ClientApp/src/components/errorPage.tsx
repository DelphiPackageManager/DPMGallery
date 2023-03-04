import { useRouteError } from "react-router";

export default function ErrorPage() {
  const error: any = useRouteError();

  return (
    <div>
      <h1>Oops!</h1>
      <p>An unexpected error occured</p>
      <p>
        <i>{error.statusText || error.message}</i>
      </p>
    </div>
  );
}
