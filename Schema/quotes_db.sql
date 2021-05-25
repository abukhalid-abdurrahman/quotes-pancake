CREATE DATABASE db_quotes
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'English_United States.1252'
    LC_CTYPE = 'English_United States.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;
	

CREATE TABLE public.authors
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name character varying(50) COLLATE pg_catalog."default" NOT NULL,
    removed boolean NOT NULL DEFAULT false,
    CONSTRAINT authors_pkey PRIMARY KEY (id)
);
		
CREATE TABLE public.tokens
(
	author_id integer NOT NULL,
    token character varying(64) NOT NULL,
    CONSTRAINT fk_token_user FOREIGN KEY (author_id)
        REFERENCES public.authors (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);

CREATE TABLE public.categories
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    category character varying(50) NOT NULL,
	removed boolean NOT NULL DEFAULT false,
    insertdate date NOT NULL DEFAULT CURRENT_DATE,
    CONSTRAINT pk_categories PRIMARY KEY (id),
);

CREATE TABLE public.quotes
(
    quote character varying(1000) COLLATE pg_catalog."default" NOT NULL,
    id integer NOT NULL,
    category_id integer NOT NULL,
    author_id integer NOT NULL,
    "insertDate" date NOT NULL DEFAULT CURRENT_DATE,
    "updateDate" date,
    removed boolean NOT NULL DEFAULT false,
    CONSTRAINT quotes_pkey PRIMARY KEY (id),
    CONSTRAINT fk_quote_author FOREIGN KEY (author_id)
        REFERENCES public.authors (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_quote_category FOREIGN KEY (category_id)
        REFERENCES public.categories (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

CREATE TABLE public.quotes_history
(
    quote_id integer NOT NULL,
    id integer NOT NULL,
    "insertDate" date NOT NULL DEFAULT CURRENT_DATE,
    action character varying(15) COLLATE pg_catalog."default" NOT NULL,
    author_id integer NOT NULL,
    CONSTRAINT quotes_history_pkey PRIMARY KEY (id),
    CONSTRAINT fk_history_author FOREIGN KEY (author_id)
        REFERENCES public.authors (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_history_quote FOREIGN KEY (quote_id)
        REFERENCES public.quotes (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

ALTER TABLE public.authors
    OWNER to postgres;
	
ALTER TABLE public.tokens
    OWNER to postgres;
	
ALTER TABLE public.categories
    OWNER to postgres;

ALTER TABLE public.quotes
    OWNER to postgres;
	
ALTER TABLE public.quotes_history
    OWNER to postgres;