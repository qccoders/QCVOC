--
-- PostgreSQL database dump
--

-- Dumped from database version 10.4 (Ubuntu 10.4-0ubuntu0.18.04)
-- Dumped by pg_dump version 10.4 (Ubuntu 10.4-0ubuntu0.18.04)

-- READ: Search and replace qcvoc with your system username.

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: accounts; Type: TABLE; Schema: public; Owner: qcvoc
--

CREATE TABLE public.accounts (
    id uuid NOT NULL,
    name text NOT NULL,
    passwordhash text NOT NULL,
    role text NOT NULL
);


ALTER TABLE public.accounts OWNER TO qcvoc;

--
-- Name: patrons; Type: TABLE; Schema: public; Owner: qcvoc
--

CREATE TABLE public.patrons (
    id uuid NOT NULL,
    memberid integer,
    firstname text NOT NULL,
    lastname text NOT NULL,
    address text NOT NULL,
    primaryphone text NOT NULL,
    secondaryphone text NOT NULL,
    email text NOT NULL,
    enrollmentdate timestamp(0) without time zone NOT NULL,
	lastupdatedate timestamp(0) without time zone NOT NULL,
	lastupdateby text NOT NULL
);


ALTER TABLE public.patrons OWNER TO qcvoc;

--
-- Name: refreshtokens; Type: TABLE; Schema: public; Owner: qcvoc
--

CREATE TABLE public.refreshtokens (
    tokenid uuid NOT NULL,
    issued timestamp(0) without time zone NOT NULL,
    expires timestamp(0) without time zone NOT NULL,
    accountid uuid NOT NULL
);


ALTER TABLE public.refreshtokens OWNER TO qcvoc;

--
-- Name: test; Type: TABLE; Schema: public; Owner: qcvoc
--

CREATE TABLE public.test (
    id integer
);


ALTER TABLE public.test OWNER TO qcvoc;

--
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: qcvoc
--

COPY public.accounts (id, name, passwordhash, role) FROM stdin;
\.


--
-- Data for Name: patrons; Type: TABLE DATA; Schema: public; Owner: qcvoc
--

COPY public.patrons (id, memberid, firstname, lastname, address, primaryphone, secondaryphone, email, enrollmentdate) FROM stdin;
\.


--
-- Data for Name: refreshtokens; Type: TABLE DATA; Schema: public; Owner: qcvoc
--

COPY public.refreshtokens (tokenid, issued, expires, accountid) FROM stdin;
\.


--
-- Data for Name: test; Type: TABLE DATA; Schema: public; Owner: qcvoc
--

COPY public.test (id) FROM stdin;
\.


--
-- Name: accounts account_pkey; Type: CONSTRAINT; Schema: public; Owner: qcvoc
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT account_pkey PRIMARY KEY (id);


--
-- Name: patrons patrons_pkey; Type: CONSTRAINT; Schema: public; Owner: qcvoc
--

ALTER TABLE ONLY public.patrons
    ADD CONSTRAINT patrons_pkey PRIMARY KEY (id);


--
-- Name: refreshtokens refreshtokens_pkey; Type: CONSTRAINT; Schema: public; Owner: qcvoc
--

ALTER TABLE ONLY public.refreshtokens
    ADD CONSTRAINT refreshtokens_pkey PRIMARY KEY (accountid);


--
-- Name: refreshtokens unique_id; Type: CONSTRAINT; Schema: public; Owner: qcvoc
--

ALTER TABLE ONLY public.refreshtokens
    ADD CONSTRAINT unique_id UNIQUE (tokenid);


--
-- PostgreSQL database dump complete
--

