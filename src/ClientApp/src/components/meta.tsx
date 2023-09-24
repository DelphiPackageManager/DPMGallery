import { ReactNode } from "react";
import { Helmet } from "react-helmet-async";

interface IMetaProps {
  url?: string;
  title?: string;
  description?: string;
  imgUrl?: string;
  canonical?: string;
  children?: ReactNode;
}

//simple wrapper around Helmet

const Meta = ({ url, title, description, imgUrl, canonical, children }: IMetaProps) => {
  const img = imgUrl ? imgUrl : "https://delphi.dev/img/favicon.png";

  return (
    <>
      <Helmet title={title}>
        {description && <meta name="description" content={description} />}
        {url && <meta property="og:url" content={url} />}
        <meta property="og:type" content="website" />
        {description && <meta property="og:description" content={description} />}
        <meta property="og:image" content={img} />
        {canonical && <link rel="canonical" href={canonical} />}
        {children}
      </Helmet>
    </>
  );
};

export default Meta;
