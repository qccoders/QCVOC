--
-- PostgreSQL database dump
--

-- Dumped from database version 10.3
-- Dumped by pg_dump version 10.3

-- Started on 2018-05-19 22:42:32 CDT

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
-- TOC entry 1 (class 3079 OID 13638)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 3520 (class 0 OID 0)
-- Dependencies: 1
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 196 (class 1259 OID 16394)
-- Name: accounts; Type: TABLE; Schema: public; Owner: b4d
--

CREATE TABLE public.accounts (
    id uuid NOT NULL,
    name text NOT NULL,
    passwordhash text NOT NULL,
    role text NOT NULL
);


ALTER TABLE public.accounts OWNER TO b4d;

--
-- TOC entry 197 (class 1259 OID 16409)
-- Name: refreshtokens; Type: TABLE; Schema: public; Owner: b4d
--

CREATE TABLE public.refreshtokens (
    tokenid uuid NOT NULL,
    issued timestamp(0) without time zone NOT NULL,
    expires timestamp(0) without time zone NOT NULL,
    accountid uuid NOT NULL
);


ALTER TABLE public.refreshtokens OWNER TO b4d;

--
-- TOC entry 3387 (class 2606 OID 16401)
-- Name: accounts account_pkey; Type: CONSTRAINT; Schema: public; Owner: b4d
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT account_pkey PRIMARY KEY (id);


--
-- TOC entry 3389 (class 2606 OID 16413)
-- Name: refreshtokens refreshtokens_pkey; Type: CONSTRAINT; Schema: public; Owner: b4d
--

ALTER TABLE ONLY public.refreshtokens
    ADD CONSTRAINT refreshtokens_pkey PRIMARY KEY (accountid);


--
-- TOC entry 3391 (class 2606 OID 16415)
-- Name: refreshtokens unique_id; Type: CONSTRAINT; Schema: public; Owner: b4d
--

ALTER TABLE ONLY public.refreshtokens
    ADD CONSTRAINT unique_id UNIQUE (tokenid);


-- Completed on 2018-05-19 22:42:32 CDT

--
-- PostgreSQL database dump complete
--

