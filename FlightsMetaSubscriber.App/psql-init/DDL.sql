create table users
(
    id         bigint not null
        constraint user_pk
            primary key,
    username   varchar,
    active     boolean,
    created_dt timestamp default (now() AT TIME ZONE 'utc'::text)
);

alter table users
    owner to postgres;

create table subscription
(
    id                 serial
        constraint subscription_pk
            primary key,
    user_id            bigint
        constraint subscription_users_id_fk
            references users,
    departure_min_date date,
    departure_max_date date,
    return_min_date    date,
    return_max_date    date,
    only_direct        boolean,
    baggage            boolean,
    active             boolean,
    created_dt         timestamp default (now() AT TIME ZONE 'utc'::text)
);

alter table subscription
    owner to postgres;

create table search_result
(
    id                serial
        constraint search_result_pk
            primary key,
    subscription_id   integer   not null
        constraint search_result_subscription_id_fk
            references subscription,
    search_dt         timestamp not null,
    origin            varchar   not null,
    destination       varchar   not null,
    departure_at      timestamp,
    return_at         timestamp,
    price             numeric,
    ticket_link       varchar,
    dt_offset         varchar,
    number_of_changes integer,
    baggage           boolean
);

alter table search_result
    owner to postgres;

create table iata_object
(
    code varchar not null
        constraint iata_object_pk
            primary key,
    name varchar
);

alter table iata_object
    owner to postgres;

create table origin
(
    subscribe_id integer not null
        constraint origin_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null
        constraint origin_iata_object_code_fk
            references iata_object,
    iata_name    varchar,
    constraint origin_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table origin
    owner to postgres;

create table destination
(
    subscribe_id integer not null
        constraint destination_subscription_id_fk
            references subscription,
    user_id      bigint  not null,
    iata_code    varchar not null
        constraint destination_iata_object_code_fk
            references iata_object,
    iata_name    varchar,
    constraint destination_pk
        primary key (subscribe_id, user_id, iata_code)
);

alter table destination
    owner to postgres;
